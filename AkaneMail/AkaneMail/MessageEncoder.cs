using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AkaneMail
{
    class MessageEncoder
    {
        private Encoding encoding;
        private List<string> HeaderFields = new List<string>();
        private string MessageBody = "";

        public MessageEncoder(string name)
        {
            this.encoding = Encoding.GetEncoding(name);
        }

        public Encoding getEncoding()
        {
            return encoding;
        }

        public string getResult()
        {
            string result = "";
            HeaderFields.ForEach(field =>
            {
                result += field + "\r\n";
            });
            result += "\r\n";
            result += MessageBody;
            return result;
        }

        public void addHeader(string MessageHeader)
        {
            foreach (string Field in MessageHeader.Replace("\r\n", "\n").Split('\n').Where(i => i != ""))
            {
                if (Field[0] == ' ')
                {
                    HeaderFields[HeaderFields.Count - 1] += Field;
                }
                else
                {
                    int pos = Field.IndexOf(':');
                    string Name = Field.Substring(0, pos);
                    string Value = Field.Substring(pos + 1).TrimStart();
                    addHeaderField(Name, Value);
                }
            }
        }

        public void addHeaderField(string FieldName, string FieldValue)
        {
            switch (FieldName.ToLower())
            {
                case "subject":
                case "from":
                case "to":
                    if (Encoding.GetEncoding("SHIFT_JIS").GetBytes(FieldValue).Length != FieldValue.Length)
                    {
                        // 全角文字を含んでいる場合、エンコードする必要がある
                        // この判定方法は気に入らないので、要修正
                        HeaderFields.Add(makeEncodedField(FieldName, FieldValue));
                    }
                    else
                    {
                        // 半角文字の場合
                        // これでアスキー文字が判定できているか、不明
                        HeaderFields.Add(makeHeaderField(FieldName, FieldValue));
                    }
                    break;
                default:
                    HeaderFields.Add(makeHeaderField(FieldName, FieldValue));
                    break;
            }
        }

        public void addMessageBody(string MessageBody)
        {
            this.MessageBody = MessageBody;
        }

        private string makeHeaderField(string FieldName, string FieldValue)
        {
            return FieldName + ": " + FieldValue;
        }

        private string makeEncodedField(string FieldName, string FieldValue)
        {
            return makeHeaderField(FieldName, encodeText(encoding.WebName, "B", FieldValue));
        }

        private string encodeText(string charset, string method, string text)
        {
            const string open = "=?";
            const string close = "?=";
            const string separator = "?";

            string encoded = "";
            switch (method)
            {
                case "B":
                    encoded = Convert.ToBase64String(encoding.GetBytes(text));
                    break;
                //case "Q":
                //    break;
                default:
                    throw new Exception("Unknown encoding method.");
            }

            return open + charset + separator + method + separator + encoded + close;
        }
    }
}
