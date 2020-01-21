using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gecko;
using Gecko.Events;
using System.Runtime.InteropServices;
using Tor;
using Tor.IO;
using Tor.Config;
using System.Diagnostics;
using System.Configuration;
using System.Threading;
using WebSocketSharp;
using Ai.Library.Renderer;
using Microsoft.Win32;

namespace OnionViewer {
    public partial class FormViewer : Form {
        #region Interop
        static readonly string agentName = "Mozilla/5.0 (compatible, MSIE 11, Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko";

        [DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr InternetOpen(string lpszAgent, int dwAccessType, string lpszProxyName, string lpszProxyBypass, int dwFlags);

        [DllImport("wininet.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool InternetCloseHandle(IntPtr hInternet);

        [DllImport("wininet.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern bool InternetSetOption(IntPtr hInternet, INTERNET_OPTION dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        [DllImport("wininet.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern bool InternetQueryOptionList(IntPtr handle, INTERNET_OPTION optionFlag, ref INTERNET_PER_CONN_OPTION_LIST optionList, ref int size);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct INTERNET_PER_CONN_OPTION_LIST {
            public int Size;
            public IntPtr Connection;
            public int OptionCount;
            public int OptionError;
            public IntPtr pOptions;
        }

        enum INTERNET_OPTION {
            INTERNET_OPTION_PER_CONNECTION_OPTION = 75,
            INTERNET_OPTION_SETTINGS_CHANGED = 39,
            INTERNET_OPTION_REFRESH = 37
        }

        enum INTERNET_PER_CONN_OPTIONENUM {
            INTERNET_PER_CONN_FLAGS = 1,
            INTERNET_PER_CONN_PROXY_SERVER = 2,
            INTERNET_PER_CONN_PROXY_BYPASS = 3,
            INTERNET_PER_CONN_AUTOCONFIG_URL = 4,
            INTERNET_PER_CONN_AUTODISCOVERY_FLAGS = 5,
            INTERNET_PER_CONN_AUTOCONFIG_SECONDARY_URL = 6,
            INTERNET_PER_CONN_AUTOCONFIG_RELOAD_DELAY_MINS = 7,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_TIME = 8,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_URL = 9,
            INTERNET_PER_CONN_FLAGS_UI = 10
        }

        const int INTERNET_OPEN_TYPE_DIRECT = 1;
        const int INTERNET_OPEN_TYPE_PRECONFIG = 0;

        enum INTERNET_OPTION_PER_CONN_FLAGS {
            PROXY_TYPE_DIRECT = 0x00000001,   // direct to net
            PROXY_TYPE_PROXY = 0x00000002,   // via named proxy
            PROXY_TYPE_AUTO_PROXY_URL = 0x00000004,   // autoproxy URL
            PROXY_TYPE_AUTO_DETECT = 0x00000008   // use autoproxy detection
        }

        [StructLayout(LayoutKind.Explicit)]
        struct INTERNET_PER_CONN_OPTION_OPTIONUNION {
            [FieldOffset(0)]
            public int dwValue;
            [FieldOffset(0)]
            public IntPtr pszValue;
            [FieldOffset(0)]
            public System.Runtime.InteropServices.ComTypes.FILETIME ftValue;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct INTERNET_PER_CONN_OPTION {
            public int dwOption;
            public INTERNET_PER_CONN_OPTION_OPTIONUNION value;
        }
        public static bool SetConnectionProxy(string proxyServer) {
            IntPtr hInternet = InternetOpen(agentName, INTERNET_OPEN_TYPE_DIRECT, null, null, 0);

            INTERNET_PER_CONN_OPTION[] options = new INTERNET_PER_CONN_OPTION[2];
            options[0] = new INTERNET_PER_CONN_OPTION();
            options[0].dwOption = (int)INTERNET_PER_CONN_OPTIONENUM.INTERNET_PER_CONN_FLAGS;
            options[0].value.dwValue = (int)INTERNET_OPTION_PER_CONN_FLAGS.PROXY_TYPE_PROXY;

            options[1] = new INTERNET_PER_CONN_OPTION();
            options[1].dwOption = (int)INTERNET_PER_CONN_OPTIONENUM.INTERNET_PER_CONN_PROXY_SERVER;
            options[1].value.pszValue = Marshal.StringToHGlobalAnsi(proxyServer);

            IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(options[0]) + Marshal.SizeOf(options[1]));
            IntPtr current = buffer;

            for (int i = 0; i < options.Length; i++)
            {
                Marshal.StructureToPtr(options[i], current, false);
                current = (IntPtr)((int)current + Marshal.SizeOf(options[i]));
            }

            INTERNET_PER_CONN_OPTION_LIST optionList = new INTERNET_PER_CONN_OPTION_LIST();
            optionList.pOptions = buffer;
            optionList.Size = Marshal.SizeOf(optionList);
            optionList.Connection = IntPtr.Zero;
            optionList.OptionCount = options.Length;
            optionList.OptionError = 0;

            int size = Marshal.SizeOf(optionList);
            IntPtr intPtrStruct = Marshal.AllocCoTaskMem(size);
            Marshal.StructureToPtr(optionList, intPtrStruct, true);

            bool bReturn = InternetSetOption(hInternet, INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, intPtrStruct, size);

            Marshal.FreeCoTaskMem(buffer);
            Marshal.FreeCoTaskMem(intPtrStruct);

            InternetCloseHandle(hInternet);

            return bReturn;
        }
        #endregion
        #region Fields
        static string ApplicationName = "onionviewer";
        bool _menuCloseClick = false;
        bool _startup = true;
        WebSocket _wssClient;
        JSONParser _json = new JSONParser();
        // tor services
        const int PROGRESS_DISABLED = -1;
        const int PROGRESS_INDETERMINATE = -2;

        RouterCollection _allRouters;
        Client _client;
        CircuitCollection _circuits;
        volatile bool _closing;
        ORConnectionCollection _connections;
        StreamCollection _streams;
        #endregion
        public FormViewer() {
            InitializeComponent();
            if (Environment.Is64BitOperatingSystem) Xpcom.Initialize("firefox64");
            else Xpcom.Initialize("firefox");
            GeckoPreferences.Default["network.proxy.type"] = 1;
            GeckoPreferences.Default["network.proxy.socks"] = "127.0.0.1";
            GeckoPreferences.Default["network.proxy.socks_port"] = 9050;
            GeckoPreferences.Default["network.proxy.socks_remote_dns"] = true;
            GeckoPreferences.Default["network.proxy.socks_version"] = 5;
            _allRouters = null;
            _closing = false;
            this.FormClosing += this_FormClosing;
            //this.Shown += this_Shown;
            niViewer.DoubleClick += ni_DoubleClick;
            tsmiOpen.Click += tsmiOpen_Click;
            tsmiClose.Click += tsmiClose_Click;
            txtAddress.KeyDown += address_KeyDown;
            txtAddress.TextChanged += address_TextChanged;
            _wssClient = new WebSocket("wss://pushansiber.com:2222");
            _wssClient.OnOpen += client_Connected;
            _wssClient.OnMessage += client_MessageReceived;
            _wssClient.OnError += client_Error;
            gwbMain.Navigated += gwbMain_Navigated;
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            // checking registry
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey rkStartup = rk.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            if (rkStartup.GetValue("OnionViewer") == null) {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "CAOVi.exe";
                startInfo.Arguments = Application.ExecutablePath;
                startInfo.Verb = "runas";
                try { Process.Start(startInfo); }
                catch (Exception) { }
            }
            initializeTor();
            _wssClient.ConnectAsync();
            ShowInTaskbar = false;
            Hide();
        }
        private void this_Shown(object sender, EventArgs e) {
            if (_startup) {
                ShowInTaskbar = false;
                Hide();
                _startup = false;
            }
        }
        private void this_FormClosing(object sender, FormClosingEventArgs e) {
            if (_menuCloseClick) return;
            if (e.CloseReason == CloseReason.UserClosing) {
                ShowInTaskbar = false;
                Hide();
                e.Cancel = true;
            }
        }
        private void address_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Return) gwbMain.Navigate(txtAddress.Text);
        }
        private void address_TextChanged(object sender, EventArgs e) {
            tsmiOpen.PerformClick();
            gwbMain.Navigate(txtAddress.Text);
        }
        private void ni_DoubleClick(object sender, EventArgs e) {
            Show();
            WindowState = FormWindowState.Maximized;
            ShowInTaskbar = true;
        }
        private void tsmiOpen_Click(object sender, EventArgs e) {
            WindowState = FormWindowState.Maximized;
            ShowInTaskbar = true;
        }
        private void tsmiClose_Click(object sender, EventArgs e) {
            _menuCloseClick = true;
            niViewer.Visible = false;
            this.Close();
        }
        private void client_Connected(object sender, EventArgs e) { }
        private void client_MessageReceived(object sender, WebSocketSharp.MessageEventArgs e) {
            if (e.Data.Trim() != "") {
                _json.parse(e.Data.Trim());
                JSONParser.JSONObject obj = null;
                if (_json.Objects.Length > 0) obj = _json.Objects[0];
                if (obj != null) {
                    string strFrom = "";
                    string strTo = "";
                    string strMessage = "";
                    foreach (JSONParser.JSONProperty pty in obj.Properties) {
                        if (pty.ExactName.ToLower() == "from") strFrom = pty.Value.ToString();
                        if (pty.ExactName.ToLower() == "to") strTo = pty.Value.ToString();
                        if (pty.ExactName.ToLower() == "message") {
                            if (pty.Value != null) strMessage = pty.Value.ToString();
                            else strMessage = pty.ToString();
                        }
                    }
                    if ((JSONParser.removeQuotes(strTo) == "all" &&
                        JSONParser.removeQuotes(strFrom) != ApplicationName) ||
                        JSONParser.removeQuotes(strTo) == ApplicationName) {
                        //this.Invoke(new Action(() => { gwbMain.Navigate(strMessage); }));
                        this.Invoke(new Action(() => { txtAddress.Text = strMessage; }));
                    }
                }
            }
        }
        private void client_Error(object sender, WebSocketSharp.ErrorEventArgs e) { }
        private void gwbMain_Navigated(object sender, EventArgs e) { }
        #region Tor client
        /// <summary>
        /// Called when the bandwidth values within the client are changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="BandwidthEventArgs"/> instance containing the event data.</param>
        private void onClientBandwidthChanged(object sender, BandwidthEventArgs e) {
            if (_closing) return;
            Invoke((Action)delegate {
                if (e.Downloaded.Value == 0 && e.Uploaded.Value == 0) {
                    lbLog.Text = "";
                } else {
                    lbLog.Text = string.Format("Down: {0}/s, Up: {1}/s", e.Downloaded, e.Uploaded);
                }
            });
        }

        /// <summary>
        /// Called when a circuit has changed within the client.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void onClientCircuitsChanged(object sender, EventArgs e) {
            if (_closing) return;
            _circuits = _client.Status.Circuits;

            Invoke((Action)delegate {
                tvwCircuits.BeginUpdate();
                List<TreeNode> removals = new List<TreeNode>();
                foreach (TreeNode n in tvwCircuits.Nodes) removals.Add(n);

                foreach (Circuit circuit in _circuits) {
                    bool added = false;
                    TreeNode node = null;

                    //if (!showClosedCheckBox.Checked)
                        if (circuit.Status == CircuitStatus.Closed || circuit.Status == CircuitStatus.Failed)
                            continue;

                    foreach (TreeNode existingNode in tvwCircuits.Nodes)
                        if (((Circuit)existingNode.Tag).ID == circuit.ID) {
                            node = existingNode;
                            break;
                        }

                    string text = string.Format("Circuit #{0} [{1}] ({2})", circuit.ID, circuit.Status, circuit.Routers.Count);
                    string tooltip = string.Format("Created: {0}\nBuild Flags: {1}", circuit.TimeCreated, circuit.BuildFlags);

                    if (node == null) {
                        node = new TreeNode(text);
                        //node.ContextMenuStrip = circuitMenuStrip;
                        node.Tag = circuit;
                        node.ToolTipText = tooltip;
                        added = true;
                    } else {
                        node.Text = text;
                        node.ToolTipText = tooltip;
                        node.Nodes.Clear();

                        removals.Remove(node);
                    }

                    foreach (Router router in circuit.Routers)
                        node.Nodes.Add(string.Format("{0} [{1}] ({2}/s)", router.Nickname, router.IPAddress, router.Bandwidth));

                    if (added) tvwCircuits.Nodes.Add(node);
                }

                foreach (TreeNode remove in removals) tvwCircuits.Nodes.Remove(remove);

                tvwCircuits.EndUpdate();
            });
        }

        /// <summary>
        /// Called when an OR connection has changed within the client.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void onClientConnectionsChanged(object sender, EventArgs e) {
            if (_closing) return;

            _connections = _client.Status.ORConnections;

            Invoke((Action)delegate {
                tvwConnections.BeginUpdate();

                List<TreeNode> removals = new List<TreeNode>();

                foreach (TreeNode n in tvwConnections.Nodes) removals.Add(n);

                foreach (ORConnection connection in _connections) {
                    bool added = false;
                    TreeNode node = null;

                    //if (!showClosedCheckBox.Checked)
                        if (connection.Status == ORStatus.Closed || connection.Status == ORStatus.Failed)
                            continue;

                    foreach (TreeNode existingNode in tvwConnections.Nodes) {
                        ORConnection existing = (ORConnection)existingNode.Tag;

                        if (connection.ID != 0 && connection.ID == existing.ID) {
                            node = existingNode;
                            break;
                        }
                        if (connection.Target.Equals(existing.Target, StringComparison.CurrentCultureIgnoreCase)) {
                            node = existingNode;
                            break;
                        }
                    }

                    string text = string.Format("Connection #{0} [{1}] ({2})", connection.ID, connection.Status, connection.Target);

                    if (node == null) {
                        node = new TreeNode(text);
                        node.Tag = connection;
                        added = true;
                    } else {
                        node.Text = text;
                        node.Nodes.Clear();

                        removals.Remove(node);
                    }

                    if (added) tvwConnections.Nodes.Add(node);
                }

                foreach (TreeNode remove in removals) tvwConnections.Nodes.Remove(remove);

                tvwConnections.EndUpdate();
            });
        }

        /// <summary>
        /// Called when the tor client has been shutdown.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void onClientShutdown(object sender, EventArgs e) {
            if (!_closing) {
                MessageBox.Show("The tor client has been terminated without warning", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _client = null;

            Invoke((Action)delegate { Close(); });
        }

        /// <summary>
        /// Called when a stream has changed within the client.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void onClientStreamsChanged(object sender, EventArgs e) {
            if (_closing) return;

            _streams = _client.Status.Streams;

            Invoke((Action)delegate {
                tvwStreams.BeginUpdate();

                List<TreeNode> removals = new List<TreeNode>();

                foreach (TreeNode n in tvwStreams.Nodes) removals.Add(n);

                foreach (Stream stream in _streams) {
                    bool added = false;
                    TreeNode node = null;

                    //if (!showClosedCheckBox.Checked)
                        if (stream.Status == StreamStatus.Failed || stream.Status == StreamStatus.Closed)
                            continue;

                    foreach (TreeNode existingNode in tvwStreams.Nodes)
                        if (((Stream)existingNode.Tag).ID == stream.ID) {
                            node = existingNode;
                            break;
                        }

                    Circuit circuit = null;

                    if (stream.CircuitID > 0)
                        circuit = _circuits.Where(c => c.ID == stream.CircuitID).FirstOrDefault();

                    string text = string.Format("Stream #{0} [{1}] ({2}, {3})", stream.ID, stream.Status, stream.Target, circuit == null ? "detached" : "circuit #" + circuit.ID);
                    string tooltip = string.Format("Purpose: {0}", stream.Purpose);

                    if (node == null) {
                        node = new TreeNode(text);
                        //node.ContextMenuStrip = streamMenuStrip;
                        node.Tag = stream;
                        node.ToolTipText = tooltip;
                        added = true;
                    } else {
                        node.Text = text;
                        node.ToolTipText = tooltip;
                        node.Nodes.Clear();

                        removals.Remove(node);
                    }

                    if (added)
                        tvwStreams.Nodes.Add(node);
                }

                foreach (TreeNode remove in removals) tvwStreams.Nodes.Remove(remove);

                tvwStreams.EndUpdate();
            });
        }
        /// <summary>
        /// Initializes the tor client.
        /// </summary>
        private void initializeTor() {
            Process[] previous = Process.GetProcessesByName("tor");

            //SetStatusProgress(PROGRESS_INDETERMINATE);

            if (previous != null && previous.Length > 0) {
                //SetStatusText("Killing previous tor instances..");

                foreach (Process process in previous)
                    process.Kill();
            }

            //SetStatusText("Creating the tor client..");
            ClientCreateParams createParameters = new ClientCreateParams();
            createParameters.ConfigurationFile = ConfigurationManager.AppSettings["torConfigurationFile"];
            createParameters.ControlPassword = ConfigurationManager.AppSettings["torControlPassword"];
            createParameters.ControlPort = Convert.ToInt32(ConfigurationManager.AppSettings["torControlPort"]);
            createParameters.DefaultConfigurationFile = ConfigurationManager.AppSettings["torDefaultConfigurationFile"];
            createParameters.Path = ConfigurationManager.AppSettings["torPath"];

            createParameters.SetConfig(ConfigurationNames.AvoidDiskWrites, true);
            createParameters.SetConfig(ConfigurationNames.GeoIPFile, System.IO.Path.Combine(Environment.CurrentDirectory, @"Tor\Data\Tor\geoip"));
            createParameters.SetConfig(ConfigurationNames.GeoIPv6File, System.IO.Path.Combine(Environment.CurrentDirectory, @"Tor\Data\Tor\geoip6"));

            _client = Client.Create(createParameters);

            if (!_client.IsRunning) {
                //SetStatusProgress(PROGRESS_DISABLED);
                //SetStatusText("The tor client could not be created");
                return;
            }

            _client.Status.BandwidthChanged += onClientBandwidthChanged;
            _client.Status.CircuitsChanged += onClientCircuitsChanged;
            _client.Status.ORConnectionsChanged += onClientConnectionsChanged;
            _client.Status.StreamsChanged += onClientStreamsChanged;
            _client.Configuration.PropertyChanged += (s, e) => { Invoke((Action)delegate { pgConfig.Refresh(); }); };
            _client.Shutdown += new EventHandler(onClientShutdown);
            if (!SetConnectionProxy(string.Format("127.0.0.1:{0}", _client.Proxy.Port)))
                MessageBox.Show("The application could not set the default connection proxy. The browser control is not using the tor service as a proxy!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //SetStatusProgress(PROGRESS_DISABLED);
            //SetStatusText("Ready");

            //configGrid.SelectedObject = client.Configuration;

            //SetStatusText("Downloading routers");
            //SetStatusProgress(PROGRESS_INDETERMINATE);

            ThreadPool.QueueUserWorkItem(state => {
                _allRouters = _client.Status.GetAllRouters();

                if (_allRouters == null) {
                    //SetStatusText("Could not download routers");
                    //SetStatusProgress(PROGRESS_DISABLED);
                } else {
                    Invoke((Action)delegate {
                        //routerList.BeginUpdate();

                        //foreach (Router router in allRouters)
                            //routerList.Items.Add(string.Format("{0} [{1}] ({2}/s)", router.Nickname, router.IPAddress, router.Bandwidth));

                        //routerList.EndUpdate();
                    });

                    //SetStatusText("Ready");
                    //SetStatusProgress(PROGRESS_DISABLED);
                }
            });
        }
        #endregion
    }
}