using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Flexlive.CQP.CSharpPlugins.Demo
{

    public partial class FormSettings : Form
    {
        public static string Serverip { get; set; }
        public static int ServerPort { get; set; }
        public static bool Fudu { get; set; }
        public FormSettings()
        {
            InitializeComponent();

            //加载标题。
            this.Text = System.Reflection.Assembly.GetAssembly(this.GetType()).GetName().Name + " 参数设置";
            this.textBox1.Text = Serverip;
            this.textBox2.Text = ServerPort.ToString();
            this.checkBox1.Checked = Fudu;
        }

        /// <summary>
        /// 退出按钮事件处理方法。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 保存按钮事件处理方法。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //参数保存处理代码
            Serverip = textBox1.Text;
            ServerPort  = int.Parse(textBox2.Text);
            Fudu = checkBox1.Checked;
            this.btnExit_Click(null, null);
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
