using Flexlive.CQP.Framework;
using System;
using System.Messaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
//添加Socket类
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace Flexlive.CQP.CSharpPlugins.Demo
{

    public class CopyToClipBoard
    {
        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="dic">请求参数定义</param>
        /// <returns></returns>
        public static string Get(string url, Dictionary<string, string> dic)
        {
            string result = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(builder.ToString());
            req.ContentType = "text/plain; charset=UTF-8";
            //添加参数
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            try
            {
                //获取内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                stream.Close();
            }
            return result;
        }
        private string ExtractKeysAndReg(string plainText)
        {
            List<string> listStrKeys = ExtractKeysFromString(plainText);
            if (listStrKeys.Count > 0)
            {
                string strKeys;


                strKeys = string.Join(",", listStrKeys.ToArray());
                try
                {
                    // Clipboard.SetText(strKeys);
                    // showInfo(strKeys);

                    //  skinTextBox1.Text += (string.Format("{0} keys have been copied to clipboard", listStrKeys.Count));
                    string stra = (string.Format("redeem {0}", strKeys));
                    //showInfo(stra);
                    return stra;

                }
                catch
                {
                    return "";
                }
            }
            else
            {
                return "";
            }

        }

        private List<string> ExtractKeysFromString(string source)
        {
            MatchCollection m = Regex.Matches(source, "([0-9A-Z]{5})(?:\\-[0-9A-Z]{5}){2,3}",
                  RegexOptions.IgnoreCase | RegexOptions.Singleline);
            List<string> result = new List<string>();
            if (m.Count > 0)
            {
                foreach (Match v in m)
                {
                    result.Add(v.Value);
                }
            }
            return result;
        }
        public void Copy(string msg)
        {
            if (!msg.Contains("-"))
            {
                return;
            }
            try
            {
                int port = FormSettings.ServerPort;
                string host = FormSettings.Serverip;

                //向服务器发送信息
                string sendStr = ExtractKeysAndReg(msg);
                Dictionary<string, string> ssstr = new Dictionary<string, string>();
                ssstr.Add("command", sendStr);
                string rsl = Get(string.Format("http://{0}:{1}/IPC", host, port), ssstr);
                CQ.SendPrivateMessage(41402913, String.Format("[{0}]运行结果：{1}", DateTime.Now.ToString(), rsl));
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("argumentNullException:{0}", e);
            }


            //Thread thread = new Thread(new ParameterizedThreadStart(cpoymessage));
            //string o = msg;
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start((object)o);
        }
        public void Commands(string msg)
        {

            try
            {
                int port = FormSettings.ServerPort;
                string host = FormSettings.Serverip;

                //向服务器发送信息
                string sendStr = msg;
                Dictionary<string, string> ssstr = new Dictionary<string, string>();
                ssstr.Add("command", sendStr);
                string rsl = Get(string.Format("http://{0}:{1}/IPC", host, port), ssstr);
                CQ.SendPrivateMessage(41402913, String.Format("[{0}]运行结果：{1}", DateTime.Now.ToString(), rsl));
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("argumentNullException:{0}", e);
            }


            //Thread thread = new Thread(new ParameterizedThreadStart(cpoymessage));
            //string o = msg;
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start((object)o);
        }
        private void Cpoymessage(object message)
        {
            string temp = (string)message;
            if (!temp.Contains("-"))
            { return; }
            try
            {
                Clipboard.SetText(temp);
            }
            catch (System.Runtime.InteropServices.ExternalException e)
            {
                Console.WriteLine("SocketException:{0}", e);
                return;
            }
        }
    }


    /// <summary>
    /// 酷Q C#版插件Demo
    /// </summary>
    public class MyPlugin : CQAppAbstract
    {
        CopyToClipBoard cp;
        /// <summary>
        /// 应用初始化，用来初始化应用的基本信息。
        /// </summary>
        public override void Initialize()
        {
            // 此方法用来初始化插件名称、版本、作者、描述等信息，
            // 不要在此添加其它初始化代码，插件初始化请写在Startup方法中。

            this.Name = "复读胸肌,抢cdkey";
            this.Version = new Version("1.0.0.0");
            this.Author = "Flexlive";
            this.Description = "复读胸肌,抢cdkey";
        }

        /// <summary>
        /// 应用启动，完成插件线程、全局变量等自身运行所必须的初始化工作。
        /// </summary>
        public override void Startup()
        {
            //完成插件线程、全局变量等自身运行所必须的初始化工作。
            FormSettings.Serverip = "106.184.7.15";
            FormSettings.ServerPort = 1242;
            FormSettings.Fudu = false;
            cp = new CopyToClipBoard();

        }

        /// <summary>
        /// 打开设置窗口。
        /// </summary>
        public override void OpenSettingForm()
        {
            // 打开设置窗口的相关代码。
            FormSettings frm = new FormSettings();
            frm.ShowDialog();
        }

        /// <summary>
        /// Type=21 私聊消息。
        /// </summary>
        /// <param name="subType">子类型，11/来自好友 1/来自在线状态 2/来自群 3/来自讨论组。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        public override void PrivateMessage(int subType, int sendTime, long fromQQ, string msg, int font)
        {
            // 处理私聊消息。
            //   CQ.SendPrivateMessage(fromQQ, String.Format("[{0}]你发的私聊消息是：{1}", CQ.ProxyType, msg));

            if (fromQQ == 41402913)
            {
                if (msg.Contains("!"))
                    cp.Commands(msg);
                else
                    cp.Copy(msg);

            }
        }

        /// <summary>
        /// Type=2 群消息。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="fromAnonymous">来源匿名者。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        public override void GroupMessage(int subType, int sendTime, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font)
        {
            if (fromGroup == 197257752)
            {
                // 处理群消息。
                var groupMember = CQ.GetGroupMemberInfo(fromGroup, fromQQ);

                cp.Copy(msg);
                //   return;
            }
            if (fromGroup == 595203733)
            {
                if (fromQQ == 1124602652 && FormSettings.Fudu)
                {
                    CQ.SendGroupMessage(fromGroup, String.Format("{0}", msg));
                }

            }

            //CQ.SendGroupMessage(fromGroup, String.Format("[{4}]{0} 你的群名片：{1}， 入群时间：{2}， 最后发言：{3}。", CQ.CQCode_At(fromQQ),
            //    groupMember.GroupCard, groupMember.JoinTime, groupMember.LastSpeakingTime, CQ.ProxyType));
            //CQ.SendGroupMessage(fromGroup, String.Format("[{0}]{1}你发的群消息是：{2}", CQ.ProxyType, CQ.CQCode_At(fromQQ), msg));
        }

        /// <summary>
        /// Type=4 讨论组消息。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromDiscuss">来源讨论组。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        public override void DiscussMessage(int subType, int sendTime, long fromDiscuss, long fromQQ, string msg, int font)
        {
            // 处理讨论组消息。
            // CQ.SendDiscussMessage(fromDiscuss, String.Format("[{0}]{1}你发的讨论组消息是：{2}", CQ.ProxyType, CQ.CQCode_At(fromQQ), msg));
        }

        /// <summary>
        /// Type=11 群文件上传事件。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="file">上传文件信息。</param>
        public override void GroupUpload(int subType, int sendTime, long fromGroup, long fromQQ, string file)
        {
            // 处理群文件上传事件。
            // CQ.SendGroupMessage(fromGroup, String.Format("[{0}]{1}你上传了一个文件：{2}", CQ.ProxyType, CQ.CQCode_At(fromQQ), file));
        }

        /// <summary>
        /// Type=101 群事件-管理员变动。
        /// </summary>
        /// <param name="subType">子类型，1/被取消管理员 2/被设置管理员。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        public override void GroupAdmin(int subType, int sendTime, long fromGroup, long beingOperateQQ)
        {
            // 处理群事件-管理员变动。
            //   CQ.SendGroupMessage(fromGroup, String.Format("[{0}]{2}({1})被{3}管理员权限。", CQ.ProxyType, beingOperateQQ, CQE.GetQQName(beingOperateQQ), subType == 1 ? "取消了" : "设置为"));
        }

        /// <summary>
        /// Type=102 群事件-群成员减少。
        /// </summary>
        /// <param name="subType">子类型，1/群员离开 2/群员被踢 3/自己(即登录号)被踢。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        public override void GroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            // 处理群事件-群成员减少。
            // CQ.SendGroupMessage(fromGroup, String.Format("[{0}]群员{2}({1}){3}", CQ.ProxyType, beingOperateQQ, CQE.GetQQName(beingOperateQQ), subType == 1 ? "退群。" : String.Format("被{0}({1})踢除。", CQE.GetQQName(fromQQ), fromQQ)));
        }

        /// <summary>
        /// Type=103 群事件-群成员增加。
        /// </summary>
        /// <param name="subType">子类型，1/管理员已同意 2/管理员邀请。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        public override void GroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            // 处理群事件-群成员增加。
            //   CQ.SendGroupMessage(fromGroup, String.Format("[{0}]群里来了新人{2}({1})，管理员{3}({4}){5}", CQ.ProxyType, beingOperateQQ, CQE.GetQQName(beingOperateQQ), CQE.GetQQName(fromQQ), fromQQ, subType == 1 ? "同意。" : "邀请。"));
        }

        /// <summary>
        /// Type=201 好友事件-好友已添加。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        public override void FriendAdded(int subType, int sendTime, long fromQQ)
        {
            // 处理好友事件-好友已添加。
            CQ.SendPrivateMessage(fromQQ, String.Format("[{0}]你好，我的朋友！", CQ.ProxyType));
        }

        /// <summary>
        /// Type=301 请求-好友添加。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">附言。</param>
        /// <param name="responseFlag">反馈标识(处理请求用)。</param>
        public override void RequestAddFriend(int subType, int sendTime, long fromQQ, string msg, string responseFlag)
        {
            // 处理请求-好友添加。
            CQ.SetFriendAddRequest(responseFlag, CQReactType.Allow, "新来的朋友");
        }

        /// <summary>
        /// Type=302 请求-群添加。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">附言。</param>
        /// <param name="responseFlag">反馈标识(处理请求用)。</param>
        public override void RequestAddGroup(int subType, int sendTime, long fromGroup, long fromQQ, string msg, string responseFlag)
        {
            // 处理请求-群添加。
            CQ.SetGroupAddRequest(responseFlag, CQRequestType.GroupAdd, CQReactType.Allow, "新群友");
        }
    }
}
