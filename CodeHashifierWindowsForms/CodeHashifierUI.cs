using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CodeHashifier
{
	internal class nativeFunctions
	{
		[DllImport("LibCodeHashifier.dll", EntryPoint="Hash", CallingConvention=CallingConvention.StdCall)]
		public static extern IntPtr Hash([MarshalAs(UnmanagedType.LPStr)] string source);
		[DllImport("LibCodeHashifier.dll", EntryPoint="GetVersion", CallingConvention=CallingConvention.StdCall)]
		public static extern IntPtr GetLibraryVersion();
	}
	public class AboutWindow : Form
	{
		private static readonly string Version = "0.0.1";
		private static readonly string BuildTime = TimeStamp.BuildTime;
		private static readonly string BuildYear = TimeStamp.BuildYear;
		private static readonly string LibraryInfo = Marshal.PtrToStringAnsi(nativeFunctions.GetLibraryVersion());
		private readonly Label AboutText = new Label();
		private readonly Label AboutIcon = new Label();
		private readonly FlowLayoutPanel Labels = new FlowLayoutPanel();
		public AboutWindow()
		{
			this.Icon = new Icon("CodeHashifier.ico");
			this.Text = "About CodeHashifier";
			this.Name = "AboutWindow";
			this.AutoSize = true;
            this.StartPosition = FormStartPosition.CenterScreen;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			
			this.SuspendLayout();
			
			Labels.SuspendLayout();
			Labels.FlowDirection = FlowDirection.LeftToRight;
			Labels.AutoSize = true;
			Labels.Dock = DockStyle.Fill;
			
			Image icon = Image.FromFile("CodeHashifier.ico");
			AboutIcon.Size = new Size(icon.Size.Height, icon.Size.Width);
			AboutIcon.ImageAlign = ContentAlignment.TopLeft;
			AboutIcon.Image = icon;
						
			AboutText.AutoSize = true;
			AboutText.MaximumSize = new Size(512, 0);
			
			AboutText.Text = $"CodeHashifier {Version}\n";
			AboutText.Text += $"Built on {BuildTime}\n\n";
			
			AboutText.Text += "Powered by:\n";
			AboutText.Text += $"{LibraryInfo}\n\n";
			
			AboutText.Text += 	"This app obfuscates your C/C++ code using random #define hashes " +
								"in order to conceal its idea from your users, competitors, employers, or simply troll your friends.\n\n";
			
			AboutText.Text += "This may also help users to bypass Cheating detectors of online competitive programming judges such as Codeforces, AtCoder...\n\n";
			
			AboutText.Text += "LICENSE\n";
			AboutText.Text += $"Copyright (C) {BuildYear} Trung Nguyen\n\n";
			AboutText.Text += 	"Permission is hereby granted, free of charge, to any person obtaining a copy of " +
								"this software and associated documentation files (the \"Software\"), " +
								"to deal in the Software without restriction, including without limitation the rights to " +
								"use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, " +
								"and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\n";
			AboutText.Text += 	"The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\n";
			AboutText.Text +=	"THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, " +
								"INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. " +
								"IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, " +
								"TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
			
			this.Labels.Controls.Add(AboutIcon);
			this.Labels.Controls.Add(AboutText);
			
			Labels.ResumeLayout();
			
			this.Controls.Add(Labels);
			
			this.ResumeLayout();
			
			//Make size const and a little bigger than text
			this.Size = new Size(this.Size.Width + 16, this.Size.Height + 16);
			this.MaximumSize = this.MinimumSize = this.Size;
		}
	}
	
    public class MainForm: Form
    {
    	private MenuStrip menu = new MenuStrip();
		private SplitContainer splitContainer = new SplitContainer();
		private TextBox InputBox = new TextBox();
		private TextBox OutputBox = new TextBox();
		
		private Panel buttons = new Panel();
		private Button startButton = new Button();
		private Button copyButton = new Button();
		
        public MainForm ()
        {
            this.Name = "CodeHashifier";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "CodeHashifier by Trung Nguyen";
        	this.Icon = new System.Drawing.Icon("CodeHashifier.ico");
        	
            this.SuspendLayout();
			        	
			//SplitContainer that contains the text boxes
        	splitContainer.SuspendLayout();
        	
	        splitContainer.Dock = DockStyle.Fill;
	        splitContainer.ForeColor = System.Drawing.SystemColors.Control;
	        splitContainer.Location = new System.Drawing.Point(0, 0);
	        splitContainer.Name = "splitContainer";
			
			splitContainer.Panel1MinSize = 128;
    		splitContainer.Panel2MinSize = 128;
    		
        	splitContainer.Size = new System.Drawing.Size(1024, 512);
			splitContainer.SplitterDistance = 512;
			
	        // This splitter moves in 10-pixel increments.
	        splitContainer.SplitterIncrement = 10;
	        splitContainer.SplitterWidth = 1;
	        splitContainer.SplitterMoved += new SplitterEventHandler(SplitterMoved);
	        splitContainer.SplitterMoving += new SplitterCancelEventHandler(SplitterMoving);
			
			//Two text boxes
	        InputBox.AcceptsReturn = true;
	        InputBox.AcceptsTab = true;
	        InputBox.Dock = DockStyle.Fill;
	        InputBox.Multiline = true;
	        InputBox.WordWrap = false;
	        InputBox.ScrollBars = ScrollBars.Both;
	        InputBox.Text = "//Your code here...";
	        
	        // textBox2
	        OutputBox.AcceptsReturn = true;
	        OutputBox.AcceptsTab = true;
	        OutputBox.Dock = DockStyle.Fill;
	        OutputBox.Multiline = true;
	        OutputBox.WordWrap = false;
	        OutputBox.ReadOnly = true;
	        OutputBox.ScrollBars = ScrollBars.Both;
	        OutputBox.Text = "H@sh1f1ed c0de here...";
	        
	        // Add a TextBox control to the left panel.
	        splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Control;
	        // Add a TextBox control to Panel1.
	        splitContainer.Panel1.Controls.Add(InputBox);
	        splitContainer.Panel1.Name = "input";
	        
	        // Add a TextBox control to the left panel.
	        splitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
	        // Add a TextBox control to Panel1.
	        splitContainer.Panel2.Controls.Add(OutputBox);
	        splitContainer.Panel2.Name = "output";
			
			//Buttons
			startButton.Text = "Hash it!";
			startButton.Dock = DockStyle.Left;
			startButton.AutoEllipsis = true;
			startButton.Click += new System.EventHandler(StartHashing);
			
			copyButton.Text = "Copy to Clipboard";
			copyButton.Dock = DockStyle.Right;
			copyButton.AutoEllipsis = true;
			copyButton.Click += new System.EventHandler(CopyResultToClipboard);
			
			//Buttons panel
			buttons.Size = new System.Drawing.Size(1024, 32);
			buttons.Dock = DockStyle.Bottom;
			buttons.Controls.Add(startButton);
			buttons.Controls.Add(copyButton);
			buttons.SizeChanged += new System.EventHandler(ButtonsSizeChanged);
			
			//Menu
			ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
			ToolStripMenuItem fileMenuOpen = new ToolStripMenuItem
				("Open", null, new EventHandler(OpenFile), Keys.Control | Keys.O);
			ToolStripMenuItem fileMenuSave = new ToolStripMenuItem
				("Save", null, new EventHandler(SaveFile), Keys.Control | Keys.S);
			fileMenu.DropDownItems.Add(fileMenuOpen);
			fileMenu.DropDownItems.Add(fileMenuSave);
			
			ToolStripMenuItem aboutMenu = new ToolStripMenuItem("About", null, new EventHandler(ShowAboutDialog));
			
			menu.MdiWindowListItem = fileMenu;
			menu.Items.Add(fileMenu);
			menu.Items.Add(aboutMenu);
			menu.Dock = DockStyle.Top;
			
			this.MainMenuStrip = menu;

        	this.Controls.Add(splitContainer);
            this.Controls.Add(buttons);
            this.Controls.Add(menu);
            
            splitContainer.ResumeLayout();
            
            this.Size = splitContainer.Size;
            this.MinimumSize = new System.Drawing.Size(512, 256);

            this.ResumeLayout();
        }
        
	    private void SplitterMoving(System.Object sender, SplitterCancelEventArgs e)
	    {
	        Cursor.Current = Cursors.Default;
	    }
	    private void SplitterMoved(System.Object sender, SplitterEventArgs e)
	    {
	    	//Users cannot move the splitter.
			splitContainer.SplitterDistance = splitContainer.Size.Width / 2;
	    }
		private void ButtonsSizeChanged(System.Object sender, EventArgs e)
		{
			startButton.Size = new System.Drawing.Size(buttons.Size.Width / 2, buttons.Size.Height);
			copyButton.Size = new System.Drawing.Size(buttons.Size.Width / 2, buttons.Size.Height);
		}
		private void ShowAboutDialog(Object sender, EventArgs e)
		{
			AboutWindow aboutWindow = new AboutWindow();
			aboutWindow.ShowDialog();
		}
		private void StartHashing(Object sender, EventArgs e)
		{
			OutputBox.Text = Hash(InputBox.Text);
		}
		private void CopyResultToClipboard(Object sender, EventArgs e)
		{
			Clipboard.Clear();
			Clipboard.SetText(OutputBox.Text);
		}
		private void SaveFile(Object sender, EventArgs e)
		{
			Stream FileStream;
			SaveFileDialog saveCodeDialog = new SaveFileDialog();
			
			saveCodeDialog.Filter = "C/C++ source files (*.c; *.cpp; *.cc; *.cxx; *.c++; *.cp)|*.c; *.cpp; *.cc; *.cxx; *.c++; *.cp|All files (*.*)|*.*";
			saveCodeDialog.FilterIndex = 1;
			saveCodeDialog.RestoreDirectory = true;
			
			if (saveCodeDialog.ShowDialog() == DialogResult.OK)
			{
				if ((FileStream = saveCodeDialog.OpenFile()) != null)
				{
					BufferedStream fastStream = new BufferedStream(FileStream);
					using (StreamWriter sw = new StreamWriter(fastStream))
					{
						sw.Write(OutputBox.Text);
					}
					fastStream.Dispose();
					FileStream.Close();	
				}
			}
		}
		private void OpenFile(Object sender, EventArgs e)
		{
			using (OpenFileDialog openCodeDialog = new OpenFileDialog())
			{
				openCodeDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				openCodeDialog.Filter = "C/C++ source files (*.c; *.cpp; *.cc; *.cxx; *.c++; *.cp)|*.c; *.cpp; *.cc; *.cxx; *.c++; *.cp|All files (*.*)|*.*";
				openCodeDialog.FilterIndex = 1;
				openCodeDialog.RestoreDirectory = true;
				
				if (openCodeDialog.ShowDialog() == DialogResult.OK)
				{
					string filePath = openCodeDialog.FileName;
					
					BufferedStream fastStream = new BufferedStream(openCodeDialog.OpenFile());
					
					using (StreamReader sr = new StreamReader(fastStream))
					{
						InputBox.Text = sr.ReadToEnd();
					}
					
					fastStream.Dispose();
				}
			}
		}
		private string Hash(string s)
		{
			IntPtr result = nativeFunctions.Hash(s);
			return Marshal.PtrToStringAnsi(result).Replace("\n", Environment.NewLine);
		}
    }

    public class Program
    {
		[STAThread]
        public static int Main(string[] args) 
		{
			if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm ());
            return 0;
        }
		[DllImport("user32.dll")]
		private static extern bool SetProcessDPIAware();
    }
}
