using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;

namespace nMail
{
	/// <summary>
	/// nMail.DLL からエラーが返ってきた場合に発生する例外
	/// </summary>
	public class nMailException: ApplicationException, ISerializable
	{
		int _error_code;
		/// <summary>
		/// エラーメッセージとエラーコードを指定して、<c>nMailException</c>クラスの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="message"></param>
		/// <param name="error_code"></param>
		public nMailException(string message, int error_code): base(message)
		{
			this._error_code = error_code;
		}
		/// <summary>
		/// シリアル化したデータを指定して、<c>nMailException</c>クラスの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public nMailException(SerializationInfo info, StreamingContext context): base(info, context)
		{
			_error_code = info.GetInt32("ErrorCode");
		}
		/// <summary>
		/// エラーコードです。数値は nMail.chm の Q＆A のエラーコード一覧も参照してみてください。
		/// </summary>
		public int ErrorCode
		{
			get
			{
				return _error_code;
			}
		}
		/// <summary>
		/// サーバーから返されたエラーメッセージです。
		/// </summary>
		public override string Message
		{
			get
			{
				return base.Message;
			}
		}
		/// <summary>
		/// 例外に関する情報を使用して SerializationInfo を設定します。
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ErrorCode", _error_code);
		}
	}

	/// <summary>
	/// Winsock 関連クラス
	/// </summary>
	public class Winsock
	{
		/// <summary>
		/// <c>Winsock</c>クラスの新規インスタンスを初期化します。
		/// </summary>
		public Winsock()
		{
		}
		/// <summary>
		/// Winsock を初期化します。
		/// </summary>
		[DllImport("nMail.DLL", EntryPoint="NMailInitializeWinSock")]
		public static extern bool Initialize();

		/// <summary>
		/// Winsock の使用を終了します。
		/// </summary>
		[DllImport("nMail.DLL", EntryPoint="NMailEndWinSock")]
		public static extern bool Done();
	}
	/// <summary>
	/// POP3メール受信クラス
	/// </summary>
	/// <remarks>
	/// <example>
	/// <para>
	/// 指定のメール番号(変数名:no)を取得し、件名と本文を表示する。
	/// 後ほど Attachment クラスで添付ファイルを展開する場合かつ text/html パートをファイルに保存したい場合、
	/// <see cref="Options.DisableDecodeBodyText()"/> もしくは <see cref="Options.DisableDecodeBodyAll()"/>を呼んでから<see cref="GetMail"/>で取得したヘッダおよび本文データを使用する必要があります。
	/// <code lang="cs">
	///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
	///		try {
	///			pop.Connect();
	///			pop.Authenticate("pop3_id", "password");
	///			pop.GetMail(no);
	///			MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}\r\n{2:s}", no, pop.Subject, pop.Body));
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
	/// Try
	/// 	pop.Connect()
	/// 	pop.Authenticate("pop3_id", "password")
	///		pop.GetMail(no)
	///		MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}" + ControlChars.CrLf + "{2:s}", no, pop.Subject, pop.Body))
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		pop.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// SSL Version 3 を使用し、指定のメール番号(変数名:no)を取得し、件名と本文を表示する。
	/// <code lang="cs">
	///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
	///		try {
	///			pop.SSL = nMail.Pop3.SSL3;
	///			pop.Connect(nMail.Pop3.StandardSslPortNo);		// over SSL/TLS のポート番号を指定して接続
	///			pop.Authenticate("pop3_id", "password");
	///			pop.GetMail(no);
	///			MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}\r\n{2:s}", no, pop.Subject, pop.Body));
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
	/// Try
	/// 	pop.SSL = nMail.Pop3.SSL3
	/// 	pop.Connect(nMail.Pop3.StandardSslPortNo)		' over SSL/TLS のポート番号を指定して接続
	/// 	pop.Authenticate("pop3_id", "password")
	///		pop.GetMail(no)
	///		MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}" + ControlChars.CrLf + "{2:s}", no, pop.Subject, pop.Body))
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		pop.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// 指定のメール番号(変数名:no)を取得し、件名と本文を表示する。添付ファイルを z:\temp に保存する。
	/// text/html パートをファイルに保存する場合、<see cref="GetMail"/> の前に <see cref="Options.EnableSaveHtmlFile()"/> を呼んでおく必要があります。
	/// 保存した text/html パートのファイル名は <see cref="HtmlFile"/> で取得できます。
	/// <code lang="cs">
	///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
	///		try {
	///			pop.Connect();
	///			pop.Authenticate("pop3_id", "password");
	///			pop.Path = @"z:\temp";
	///			pop.GetMail(no);
	///			MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}\r\n{2:s}", no, pop.Subject, pop.Body));
	///			string [] file_list = pop.GetFileNameList();
	///			if(file_list.Length == 0)
	///			{
	///				MessageBox.Show("ファイルはありません");
	///			}
	///			else
	///			{
	///				foreach(string name in file_list)
	///				{
	///					MessageBox.Show(String.Format("添付ファイル名:{0:s}", name));
	///				}
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
	/// Try
	/// 	pop.Connect()
	/// 	pop.Authenticate("pop3_id", "password")
	///		pop.Path = "z:\temp"
	///		pop.GetMail(no)
	///		MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}" + ControlChars.CrLf + "{2:s}", no, pop.Subject, pop.Body))
	///		Dim file_list As String() = pop.GetFileNameList()
	///		If file_list.Length = 0 Then
	///			MessageBox.Show("ファイルはありません")
	///		Else
	///			For Each name As String In file_list
	///				MessageBox.Show(String.Format("添付ファイル名:{0:s}", name))
	///			Next fno
	///		End If
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		pop.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	///	一時休止機能を使って指定のメール番号(変数名:no)を取得し、件名と本文を表示する。添付ファイルを z:\temp に保存する。
	/// <code lang="cs">
	///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
	///		try {
	///			pop.Connect();
	///			pop.Authenticate("pop3_id", "password");
	///			// だいたいの休止回数を得る
	///			int count = pop.GetSize(no) / (nMail.Options.SuspendSize * 1024) + 1;
	///			pop.Path = @"z:\temp";
	///			pop.GetMail(no);
	///			pop.Flag = nMail.Pop3.SuspendAttachmentFile;
	///			pop.GetMail(no);
	///			pop.Flag = nMail.Pop3.SuspendNext;
	///			while(pop.ErrorCode == nMail.Pop3.ErrorSuspendAttachmentFile)
	///			{
	///				pop.GetMail(no);
	///				// プログレスバーを進める等の処理
	///				Application.DoEvents();
	///			}
	///			MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}\r\n{2:s}", no, pop.Subject, pop.Body));
	///			string [] file_list = pop.GetFileNameList();
	///			if(file_list.Length == 0)
	///			{
	///				MessageBox.Show("ファイルはありません");
	///			}
	///			else
	///			{
	///				foreach(string name in file_list)
	///				{
	///					MessageBox.Show(String.Format("添付ファイル名:{0:s}", name));
	///				}
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	///	Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
	///	Try
	///		Dim count As Integer
	///
	///		pop.Connect()
	///		pop.Authenticate("pop3_id", "password")
	///		' だいたいの休止回数を得る
	///		count = pop.GetSize(no) \ (nMail.Options.SuspendSize * 1024) + 1
	///		pop.Path = "z:\temp"
	///		pop.Flag = nMail.Pop3.SuspendAttachmentFile
	///		pop.GetMail(no)
	///		pop.Flag = nMail.Pop3.SuspendNext
	///		Do While pop.ErrorCode = nMail.Pop3.ErrorSuspendAttachmentFile
	///			pop.GetMail(no)
	///			' プログレスバーを進める等の処理
	///			Application.DoEvents()
	///		Loop
	///		MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}" + ControlChars.CrLf + "{2:s}", no, pop.Subject, pop.Body))
	///		Dim file_list As String() = pop.GetFileNameList()
	///		If file_list.Length = 0 Then
	///			MessageBox.Show("ファイルはありません")
	///		Else
	///			For Each name As String In file_list
	///				MessageBox.Show(String.Format("添付ファイル名:{0:s}", name))
	///			Next fno
	///		End If
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		pop.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// </example>
	/// </remarks>
	public class Pop3 : IDisposable
	{
		/// <summary>
		/// ヘッダ領域のサイズです。
		/// </summary>
		protected const int HeaderSize = 32768;
		/// <summary>
		/// パス文字列のサイズです。
		/// </summary>
		protected const int MaxPath = 32768;
		/// <summary>
		/// 拡張機能用バッファサイズです。
		/// </summary>
		protected const int AttachmentTempSize = 400;
		/// <summary>
		/// ソケットエラーもしくは未接続状態です。値は -1 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Connect"/>を呼び出さずに<see cref="GetMail"/>等を、</para>
		/// <para>呼び出したり、なんらかの理由で接続が切断されるとこのエラーとなります。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorSocket = -1;
		/// <summary>
		/// 認証エラーです。値は -2 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Authenticate"/>呼び出しで認証に失敗した場合</para>
		/// <para>このエラーとなります。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorAuthenticate = -2;
		/// <summary>
		/// 番号で指定されたメールが存在しません。値は -3 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>で指定した番号のメールが存在しないとこのエラーとなります。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorInvalidNo = -3;
		/// <summary>
		/// タイムアウトエラーです。値は -4 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Options.Timeout"/>で指定した値より長い時間サーバから応答が</para>
		/// <para>ない場合、このエラーが発生します。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorTimeout = -4;
		/// <summary>
		/// 添付ファイルが開けません。値は -5 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Path"/>で指定したフォルダに添付ファイルが書き込めない</para>
		/// <para>ない場合、このエラーが発生します。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorFileOpen = -5;
		/// <summary>
		/// 分割ファイルがそろっていません。値は -6 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Flag"/>に<see cref="PartialAttachmentFile"/> を設定して</para>
		/// <para><see cref="GetMail"/>を呼び出した際に、サーバ上に分割されたメールがすべて</para>
		/// <para>揃っていない場合にこのエラーが発生します。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorPartial = -6;
		/// <summary>
		/// 添付ファイルと同名のファイルがフォルダに存在します。値は -7 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="nMail.Options.AlreadyFile"/> に <see cref="nMail.Options.FileAlreadyError"/></para>
		/// <para>を設定し、<see cref="Path"/>で指定したフォルダに添付ファイルと同じ名前のファイルが</para>
		/// <para>ある場合にこのエラーが発生します。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorFileAlready = -7;
		/// <summary>
		/// メモリ確保エラーです。値は -9 です。
		/// </summary>
		/// <remarks>
		/// 内部で文字コードを変換するためのメモリを確保できませんでした。
		/// </remarks>
		public const int ErrorMemory = -9;
		/// <summary>
		/// その他のエラーです。値は -10 です。
		/// </summary>
		public const int ErrorEtc = -10;
		/// <summary>
		/// 添付ファイル受信中でまだ残りがある状態です。値は -20 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Flag"/>に<see cref="SuspendAttachmentFile"/>または</para>
		/// <para><see cref="SuspendNext"/>を指定し<see cref="GetMail"/></para>
		/// <para>を呼び出した場合、まだメールの残りがある場合、<see cref="nMailException.ErrorCode"/></para>
		/// <para>に設定されます。</para>
		/// </remarks>
		public const int ErrorSuspendAttachmentFile = -20;
		/// <summary>
		/// 添付ファイルは存在しません。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetAttachmentFileStatus"/> 実行後、添付ファイルが存在しない場合に</para>
		/// <para><see cref="PartNo"/> に設定されます。</para>
		/// </remarks>
		public const int NoAttachmentFile = -1;
		/// <summary>
		/// 添付ファイルは存在しますが、分割されていません。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetAttachmentFileStatus"/> 実行後、添付ファイルが存在しない場合に</para>
		/// <para><see cref="PartNo"/> に設定されます。</para>
		/// </remarks>
		public const int AttachmentFile = 0;
		/// <summary>
		/// 実行に成功しました。値は 1 です。
		/// </summary>
		/// <para>各種メソッドが成功を返した場合、<see cref="nMailException.ErrorCode"/>に設定される値です。</para>
		public const int Success = 1;
		/// <summary>
		/// 分割された添付ファイルを取得します。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>の前に、<see cref="Flag"/>に設定しておきます。</para>
		/// </remarks>
		public const int PartialAttachmentFile = 1;
		/// <summary>
		/// メール本文のみ取得します。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>の前に、<see cref="Flag"/>に設定しておきます。</para>
		/// </remarks>
		public const int TextOnly = 2;
		/// <summary>
		/// 添付ファイル受信で一時休止ありの一回目に指定します。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>の前に、<see cref="Flag"/>に設定しておきます。</para>
		/// </remarks>
		public const int SuspendAttachmentFile = 4;
		/// <summary>
		/// 添付ファイル受信で一時休止ありの二回目以降に指定します。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>の前に、<see cref="Flag"/>に設定しておきます。</para>
		/// </remarks>
		public const int SuspendNext = 8;
		/// <summary>
		/// UIDL をすべて取得します。
		/// </summary>
		/// <remarks>
		/// <see cref="GetUidl"/>で指定することによって全ての UIDL を取得できます。
		/// </remarks>
		public const int UidlAll = 0;
		/// <summary>
		/// SSLv3 を使用します。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int SSL3 = 0x1000;
		/// <summary>
		/// TSLv1 を使用します。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int TLS1 = 0x2000;
		/// <summary>
		/// STARTTLS を使用します。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int STARTTLS = 0x4000;
		/// <summary>
		/// サーバ証明書が期限切れでもエラーにしません。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int IgnoreNotTimeValid = 0x0800;
		/// <summary>
		/// ルート証明書が無効でもエラーにしません。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int AllowUnknownCA = 0x0400;
		/// <summary>
		/// Common Name が一致しなくてもエラーにしません。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int IgnoreInvalidName = 0x0040;
		/// <summary>
		/// POP3 の標準ポート番号です
		/// </summary>
		public const int StandardPortNo = 110;
		/// <summary>
		/// POP3 over SSL のポート番号です
		/// </summary>
		public const int StandardSslPortNo = 995;

		/// <summary>
		/// POP3 ポート番号です。
		/// </summary>
		protected int _port = 110;
		/// <summary>
		/// ソケットハンドルです。
		/// </summary>
		protected IntPtr _socket = (IntPtr)ErrorSocket;
		/// <summary>
		/// メール数です。
		/// </summary>
		protected int _count = -1;
		/// <summary>
		/// メールサイズです。
		/// </summary>
		protected int _size = -1;
		/// <summary>
		/// ヘッダーサイズです。
		/// </summary>
		protected int _header_size = -1;
		/// <summary>
		/// 本文のサイズです。
		/// </summary>
		protected int _body_size = -1;
		/// <summary>
		/// メール受信時の設定用フラグです。
		/// </summary>
		protected int _flag = 0;
		/// <summary>
		/// 分割ファイル番号です。
		/// </summary>
		protected int _part_no = -1;
		/// <summary>
		/// エラー番号です。
		/// </summary>
		protected int _err = 0;
		/// <summary>
		/// APOP を使用するかどうかのフラグです。
		/// </summary>
		protected bool _apop = false;
		/// <summary>
		/// POP3 サーバ名です。
		/// </summary>
		protected string _host = "";
		/// <summary>
		/// POP3 ユーザー名です。
		/// </summary>
		protected string _id = "";
		/// <summary>
		/// POP3 パスワードです。
		/// </summary>
		protected string _password = "";
		/// <summary>
		/// 添付ファイル保存用のパスです。
		/// </summary>
		protected string _path = null;
		/// <summary>
		/// ヘッダフィールド名です。
		/// </summary>
		protected string _field_name = "";
		/// <summary>
		/// 本文格納バッファです。
		/// </summary>
		protected StringBuilder _body = null;
		/// <summary>
		/// 件名格納バッファです。
		/// </summary>
		protected StringBuilder _subject = null;
		/// <summary>
		/// 日付文字列保存バッファです。
		/// </summary>
		protected StringBuilder _date = null;
		/// <summary>
		/// 差出人格納バッファです。
		/// </summary>
		protected StringBuilder _from = null;
		/// <summary>
		/// ヘッダ格納バッファです。
		/// </summary>
		protected StringBuilder _header = null;
		/// <summary>
		/// 添付ファイル名格納バッファです。
		/// </summary>
		protected StringBuilder _filename = null;
		/// <summary>
		/// 添付ファイル名のリストです。
		/// </summary>
		protected string[] _filename_list = null;
		/// <summary>
		/// ヘッダフィールド内容格納バッファです。
		/// </summary>
		protected StringBuilder _field = null;
		/// <summary>
		/// 分割ファイルの ID です。
		/// </summary>
		protected StringBuilder _part_id = null;
		/// <summary>
		/// UIDL 格納バッファです。
		/// </summary>
		protected StringBuilder _uidl = null;
		/// <summary>
		/// 拡張機能用バッファです。
		/// </summary>
		protected byte[] _temp = null;
		/// <summary>
		/// Dispose 処理を行ったかどうかのフラグです。
		/// </summary>
		private bool _disposed = false;
		/// <summary>
		/// text/html パートを保存したファイルの名前です。
		/// </summary>
		protected StringBuilder _html_file = null;
		/// <summary>
		/// SSL 設定フラグです。
		/// </summary>
		protected int _ssl = 0;
		/// <summary>
		/// SSL クライアント証明書名です。
		/// </summary>
		protected string _cert_name = null;
		/// <summary>
		/// message/rfc822 パートを保存したファイルの名前です。
		/// </summary>
		protected StringBuilder _rfc822_file = null;

		/// <summary>
		/// <c>Pop3</c>クラスの新規インスタンスを初期化します。
		/// </summary>
		public Pop3()
		{
			Init();
		}
		/// <summary>
		/// <c>Pop3</c>クラスの新規インスタンスを初期化します。
		/// <param name="host_name">POP3 サーバー名</param>
		/// </summary>
		public Pop3(string host_name)
		{
			Init();
			_host = host_name;
		}
		/// <summary>
		/// 
		/// </summary>
		~Pop3()
		{
			Dispose(false);
		}
		/// <summary>
		/// <see cref="Pop3"/>によって使用されているすべてのリソースを解放します。
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// <see cref="Pop3"/>によって使用されているすべてのリソースを解放します。
		/// </summary>
		/// <param name="disposing">
		/// マネージリソースとアンマネージリソースの両方を解放する場合は<c>true</c>。
		/// アンマネージリソースだけを解放する場合は<c>false</c>。
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			if(!_disposed) {
				if(disposing)
				{
				}
				if(_socket != (IntPtr)ErrorSocket) {
					Pop3Close(_socket);
					_socket = (IntPtr)ErrorSocket;
				}
				_disposed = true;
			}
		}
		/// <summary>
		/// 初期化処理です。
		/// </summary>
		protected void Init()
		{
			_temp = new byte[AttachmentTempSize];
		}

		[DllImport("nMail.DLL", EntryPoint="NMailPop3ConnectPortNo", CharSet=CharSet.Auto)]
		protected static extern IntPtr Pop3ConnectPortNo(string Host, int Port);

		[DllImport("nMail.DLL", EntryPoint="NMailPop3ConnectSsl", CharSet=CharSet.Auto)]
		protected static extern IntPtr Pop3ConnectSsl(string Host, int Port, int Flag, string Name);

		[DllImport("nMail.DLL", EntryPoint="NMailPop3Close")]
		protected static extern int Pop3Close(IntPtr Socket);

		[DllImport("nMail.DLL", EntryPoint="NMailPop3Authenticate", CharSet=CharSet.Auto)]
		protected static extern int Pop3Authenticate(IntPtr Socket, string Id, string Pass, bool APopFlag);

		[DllImport("nMail.DLL", EntryPoint="NMailPop3GetMailStatus", CharSet=CharSet.Auto)]
		protected static extern int Pop3GetMailStatus(IntPtr Socket, int No, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, bool SizeFlag);

		[DllImport("nMail.DLL", EntryPoint="NMailPop3GetMailSize")]
		protected static extern int Pop3GetMailSize(IntPtr Socket, int No);

		[DllImport("nMail.DLL", EntryPoint="NMailPop3GetMail", CharSet=CharSet.Auto)]
		protected static extern int Pop3GetMail(IntPtr Socket, int No, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, StringBuilder Body, string Path, StringBuilder FileName);

		[DllImport("nMail.DLL", EntryPoint="NMailPop3GetMailEx", CharSet=CharSet.Auto)]
		protected static extern int Pop3GetMailEx(IntPtr Socket, int No, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, StringBuilder Body, string Path, StringBuilder FileName, byte[] Temp, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailPop3DeleteMail")]
		protected static extern int Pop3DeleteMail(IntPtr Socket, int No);

		[DllImport("nMail.DLL", EntryPoint="NMailGetHeaderField", CharSet=CharSet.Auto)]
		protected static extern int Pop3GetHeaderField(StringBuilder Field, string Header, string Name, int Size);

		[DllImport("nMail.DLL", EntryPoint="NMailPop3GetUidl", CharSet=CharSet.Auto)]
		protected static extern int Pop3GetUidl(IntPtr Socket, int No, StringBuilder Id, int Max);

		[DllImport("nMail.DLL", EntryPoint="NMailPop3GetAttachmentFileStatus", CharSet=CharSet.Auto)]
		protected static extern int Pop3GetAttachmentFileStatus(IntPtr Socket, int No, StringBuilder Id, int Max);

		[DllImport("nMail.DLL", EntryPoint="NMailDecodeHeaderField", CharSet=CharSet.Auto)]
		protected static extern int Pop3DecodeHeaderField(StringBuilder Destination, string Source, int Size);

		/// <summary>
		/// ヘッダ格納用バッファのサイズを決定します。
		/// </summary>
		protected void SetHeaderSize()
		{
			if(_header_size < 0)
			{
				_header_size = Options.HeaderMax;
			}
			if(_header_size <= 0)
			{
				_header_size = HeaderSize;
				Options.HeaderMax = _header_size;
			}
		}
		/// <summary>
		/// POP3 サーバに接続します。
		/// </summary>
		/// <remarks>
		/// POP3 サーバに接続します。
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		///	<exception cref="nMailException">
		///	POP サーバーとの接続に失敗しました。
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect()
		{
			if(_port < 0 || _port > 65536) {
				throw new ArgumentOutOfRangeException();
			}
			//_socket = Pop3ConnectPortNo(_host, _port);
			_socket = Pop3ConnectSsl(_host, _port, _ssl, _cert_name);
			if(_socket == (IntPtr)ErrorSocket)
			{
				_err = ErrorSocket;
				_socket = (IntPtr)ErrorSocket;
				throw new nMailException("Connect", _err);
			}
		}
		/// <summary>
		/// POP3 サーバに接続します。
		/// </summary>
		/// <param name="host_name">POP3 サーバー名</param>
		/// <remarks>
		/// POP3 サーバに接続します。
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		///	<exception cref="nMailException">
		///	POP サーバとの接続に失敗しました。
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect(string host_name)
		{
			_host = host_name;
			Connect();
		}
		/// <summary>
		/// POP3 サーバに接続します。
		/// </summary>
		/// <param name="host_name">POP3 サーバ名</param>
		/// <param name="port_no">ポート番号</param>
		/// <remarks>
		/// POP3 サーバに接続します。
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		///	<exception cref="nMailException">
		///	POP サーバとの接続に失敗しました。
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect(string host_name, int port_no)
		{
			_host = host_name;
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// POP3 サーバに接続します。
		/// </summary>
		/// <param name="port_no">ポート番号</param>
		/// <remarks>
		/// POP3 サーバに接続します。
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		///	<exception cref="nMailException">
		///	POP サーバとの接続に失敗しました。
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect(int port_no)
		{
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// POP3 サーバとの接続を終了します。
		/// </summary>
		public void Close()
		{
			if(_socket != (IntPtr)ErrorSocket) {
				Pop3Close(_socket);
			}
			_socket = (IntPtr)ErrorSocket;
		}
		/// <summary>
		/// POP3 サーバ認証を行います。
		/// </summary>
		/// <remarks>
		/// POP3 サーバ認証を行います。
		/// </remarks>
		/// <param name="id_str">POP3 ユーザー ID</param>
		/// <param name="pass_str">POP3 パスワード</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="FormatException">
		///	ID もしくはパスワードに文字列が入っていません。
		/// </exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Authenticate(string id_str, string pass_str)
		{
			if(id_str == "" || pass_str == "") {
				throw new FormatException();
			}
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_id = id_str;
			_password = pass_str;
			_body_size = -1;
			_count = Pop3Authenticate(_socket, _id, _password, _apop);
			if(_count < 0)
			{
				_err = _count;
				throw new nMailException("Authenticate: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// POP3 サーバ認証を行います。
		/// </summary>
		/// <remarks>
		/// POP3 サーバ認証を行います。
		/// </remarks>
		/// <param name="id_str">POP3 ユーザー ID</param>
		/// <param name="pass_str">POP3 パスワード</param>
		/// <param name="apop_flag">APOP を使用するか</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="FormatException">
		///	ID もしくはパスワードに文字列が入っていません。
		/// </exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Authenticate(string id_str, string pass_str, bool apop_flag)
		{
			_apop = apop_flag;
			Authenticate(id_str, pass_str);
		}

		/// <summary>
		/// メールのステータスを取得します。
		/// </summary>
		/// <param name="no">メール番号</param>
		/// <remarks>
		/// <paramref name="no"/>パラメータでステータスを取得したいメール番号を指定します。
		/// <para>件名は<see cref="Subject"/>で取得できます。</para>
		/// <para>日付文字列は<see cref="DateString"/>で取得できます。</para>
		/// <para>差出人は<see cref="From"/>で取得できます。</para>
		/// <para>ヘッダは<see cref="Header"/>で取得できます。</para>
		/// <para>ステータス取得失敗の場合のエラー番号は<see cref="nMailException.ErrorCode"/>で取得できます。</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	メール番号が正しくありません。
		/// </exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetStatus(int no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			if(no <= 0) {
				throw new ArgumentOutOfRangeException();
			}
			SetHeaderSize();
			_subject = new StringBuilder(_header_size);
			_date = new StringBuilder(_header_size);
			_from = new StringBuilder(_header_size);
			_header = new StringBuilder(_header_size);
			_err = Pop3GetMailStatus(_socket, no, _subject, _date, _from, _header, false);
			if(_err < 0)
			{
				throw new nMailException("GetStatus: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// メールのサイズを取得します。
		/// </summary>
		/// <param name="no">メール番号</param>
		/// <remarks>
		/// <paramref name="no"/>パラメータでメールサイズを取得したいメール番号を指定します。
		/// <example>
		/// <para>
		/// メール番号(変数名:no)のメールサイズを取得する。
		/// <code lang="cs">
		///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
		///		try {
		///			pop.Connect();
		/// 		pop.Authenticate("pop3_id", "password");
		///			pop.GetSize(no);
		///			MessageBox.Show(String.Format("メール番号:{0:d},サイズ:{1:d}", no, pop.Size));
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
		/// Try
		/// 	pop.Connect()
		/// 	pop.Authenticate("pop3_id", "password")
		///		pop.GetMail(no)
		///		MessageBox.Show(String.Format("メール番号:{0:d},サイズ:{1:d}", no, pop.Size))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		pop.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	メール番号が正しくありません。
		/// </exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		/// <returns>メールサイズ</returns>
		public int GetSize(int no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			if(no <= 0) {
				throw new ArgumentOutOfRangeException();
			}
			_size = Pop3GetMailSize(_socket, no);
			if(_size < 0) {
				_err = _size;
				throw new nMailException("GetSize: " + Options.ErrorMessage, _err);
			} else {
				_body_size = _size * 2;
			}
			return _size;
		}

		/// <summary>
		/// メールを取得します。
		/// </summary>
		/// <param name="no">メール番号</param>
		/// <remarks>
		/// <paramref name="no"/>パラメータでメールを取得したいメール番号を指定します。
		/// <para>添付ファイルを保存したい場合、<see cref="Path"/>に保存したいフォルダを指定しておきます。</para>
		/// <para>拡張機能を使用したい場合、<see cref="Flag"/>で設定しておきます。</para>
		/// <para>件名は<see cref="Subject"/>で取得できます。</para>
		/// <para>日付文字列は<see cref="DateString"/>で取得できます。</para>
		/// <para>差出人は<see cref="From"/>で取得できます。</para>
		/// <para>ヘッダは<see cref="Header"/>で取得できます。</para>
		/// <para>メールサイズは<see cref="Size"/>で取得できます。</para>
		/// <para>添付ファイル名は<see cref="FileName"/>で取得できます。</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="DirectoryNotFoundException">
		///	<see cref="Path"/>で指定したフォルダが存在しません。
		/// </exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	メール番号が正しくありません。
		/// </exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetMail(int no)
		{
			if(_socket == (IntPtr)ErrorSocket)
			{
				throw new InvalidOperationException();
			}
			if(_path != null)
			{
				if(_path != "" && !Directory.Exists(_path))
				{
					throw new DirectoryNotFoundException(_path);
				}
			}
			if(no <= 0) {
				throw new ArgumentOutOfRangeException();
			}
			if((_flag & SuspendNext) != 0) {
				_err = Pop3GetMailEx(_socket, no, _subject, _date, _from, _header, _body, _path, _filename, _temp, _flag);
			} else {
				SetHeaderSize();
				_subject = new StringBuilder(_header_size);
				_date = new StringBuilder(_header_size);
				_from = new StringBuilder(_header_size);
				_header = new StringBuilder(_header_size);
				Options.FileNameMax = MaxPath;
				_filename = new StringBuilder(MaxPath);
				if(_body_size < 0)
				{
					GetSize(no);
				}
				if(_body_size > 0)
				{
					_body = new StringBuilder(_body_size);
					if(_flag != 0) {
						_err = Pop3GetMailEx(_socket, no, _subject, _date, _from, _header, _body, _path, _filename, _temp, _flag);
					}
					else
					{
						_err = Pop3GetMail(_socket, no, _subject, _date, _from, _header, _body, _path, _filename);
					}
				}
			}
			if(_err != ErrorSuspendAttachmentFile)
			{
				_body_size = -1;
				if(_filename.Length > 0) 
				{
					_filename_list = _filename.ToString().Split(Options.SplitChar);
				} 
				else 
				{
					_filename_list = null;
				}
				if(Options.SaveHtmlFile == Options.SaveHtmlFileOn) {
					GetHeaderField("X-NMAIL-HTML-FILE:");
					_html_file = _field;
				}
				if(Options.SaveRfc822File != Options.SaveRfc822FileOff) {
					GetHeaderField("X-NMAIL-RFC822-FILE:");
					_rfc822_file = _field;
				}
			}
			if(_err < 0 && _err != ErrorSuspendAttachmentFile) {
				throw new nMailException("GetMail: " + Options.ErrorMessage, _err);
			} 
		}
		/// <summary>
		/// メールを削除します。
		/// </summary>
		/// <param name="no">メール番号</param>
		/// <remarks>
		/// <para>メール削除失敗の場合のエラー番号は<see cref="nMailException.ErrorCode"/>で取得できます。</para>
		/// <example>
		/// <para>
		/// 指定のメール番号(変数名:no)を削除する。
		/// <code lang="cs">
		///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
		///		try {
		///			pop.Connect();
		/// 		pop.Authenticate("pop3_id", "password");
		///			pop.Delete(no);
		///			MessageBox.Show(String.Format("メール番号:{0:d}を削除成功", no));
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
		/// Try
		/// 	pop.Connect()
		/// 	pop.Authenticate("pop3_id", "password")
		///		pop.Delete(no)
		///		MessageBox.Show(String.Format("メール番号:{0:d}を削除成功", no))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		pop.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	メール番号が正しくありません。
		/// </exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Delete(int no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			if(no <= 0) {
				throw new ArgumentOutOfRangeException();
			}
			_err = Pop3DeleteMail(_socket, no);
			if(_err < 0)
			{
				throw new nMailException("Delete: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// 添付ファイルの存在チェックを行います。
		/// </summary>
		/// <param name="no">メール番号</param>
		/// <remarks>
		/// <para>添付ファイルの分割数は<see cref="PartNo"/>で取得できます。</para>
		/// <para><see cref="NoAttachmentFile"/>の場合添付ファイルはありません。</para>
		/// <para><see cref="AttachmentFile"/>の場合分割されていない添付ファイル付きメールです。</para>
		/// <para>1 以上の場合分割されている添付ファイルで返り値はパート番号を表します。</para>
		/// <para>1 の場合は、<see cref="Flag"/>に<see cref="PartialAttachmentFile"/> を設定して
		/// <see cref="GetMail"/>を呼び出すと添付ファイルを結合して保存可能です。</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	メール番号が正しくありません。
		/// </exception>
		/// <returns>true で添付ファイルが存在</returns>
		public bool GetAttachmentFileStatus(int no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			if(no <= 0) {
				throw new ArgumentOutOfRangeException();
			}
			SetHeaderSize();
			_part_id = new StringBuilder(_header_size);
			_part_no = Pop3GetAttachmentFileStatus(_socket, no, _part_id, _header_size);
			if(_part_no > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// メールの UIDL を取得します。
		/// </summary>
		/// <param name="no">メール番号</param>
		/// <remarks>
		/// <para>取得した UIDL は<see cref="Uidl"/>で取得できます。</para>
		/// <para><paramref name="no"/>に<see cref="UidlAll"/>を指定すると、
		/// POP3 サーバ上にあるすべてのメールの UIDL を取得します。</para>
		/// <example>
		/// <para>
		/// 指定のメール番号(変数名:no)の UIDL を取得する。
		///	<code lang="cs">
		///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
		///		try {
		///			pop.Connect();
		/// 		pop.Authenticate("pop3_id", "password");
		///			pop.GetUidl(no);
		///			MessageBox.Show("Uidl=" + pop.Uidl);
		/// 	}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
		/// Try
		/// 	pop.Connect()
		/// 	pop.Authenticate("pop3_id", "password")
		///		pop.GetUidl(no)
		///		MessageBox.Show("Uidl=" + pop.Uidl)
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		pop.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetUidl(int no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			SetHeaderSize();
			_uidl = new StringBuilder(_header_size);
			_err = Pop3GetUidl(_socket, no, _uidl, _header_size);
			if(_err < 0)
			{
				throw new nMailException("GetUidl: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// メールヘッダから指定のフィールドの内容を取得します。
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <returns>フィールドの内容</returns>
		/// <remarks>
		/// POP3 サーバとの接続とは無関係に使用できます。
		/// <para>ヘッダは、<see cref="Header"/>で設定しておきます。
		/// <see cref="GetMail"/>で受信した直後に呼び出した場合、
		/// 受信したメールのヘッダを使用します。</para>
		/// <para>取得したフィールド内容は<see cref="Field"/>で取得できます。</para>
		/// <example>
		/// <para>
		/// 指定のメール番号(変数:no)の X-Mailer ヘッダフィールドを取得する。
		///	<code lang="cs">
		///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
		///		try {
		///			pop.Connect();
		/// 		pop.Authenticate("pop3_id", "password");
		///			pop.GetMail(no);
		///			MessageBox.Show("X-Mailer:" + pop.GetHeaderField("X-Mailer:"));
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception e)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
		///	Try
		/// 	pop.Connect()
		/// 	pop.Authenticate("pop3_id", "password")
		///		pop.GetMail(no)
		///		MessageBox.Show("X-Mailer:" + pop.GetHeaderField("X-Mailer:"))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		pop.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		/// <returns>フィールドの内容</returns>
		public string GetHeaderField(string field_name)
		{
			_field_name = field_name;
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			_err = Pop3GetHeaderField(_field, _header.ToString(), _field_name, _header_size);
			if(_err < 0)
			{
				throw new nMailException("GetHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// メールヘッダから指定のフィールドの内容を取得します。
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <param name="header">ヘッダ</param>
		/// <returns>フィールドの内容</returns>
		public string GetHeaderField(string field_name, string header)
		{
			_field_name = field_name;
			_header = new StringBuilder(header);
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			_err = Pop3GetHeaderField(_field, _header.ToString(), _field_name, _header_size);
			if(_err < 0)
			{
				throw new nMailException("GetHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// MIME ヘッダフィールドの文字列をデコードします
		/// </summary>
		/// <param name="field">フィールドの文字列</param>
		/// <returns>デコードしたフィールド内容</returns>
		public string DecodeHeaderField(string field)
		{
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			_err = Pop3DecodeHeaderField(_field, field, _header_size);
			if(_err < 0)
			{
				throw new nMailException("DecodeHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// メールヘッダから指定のヘッダフィールドの内容を取得し、
		/// MIME ヘッダフィールドのデコードを行って返します
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <returns>取得したデコード済みのフィールド内容</returns>
		public string GetDecodeHeaderField(string field_name)
		{
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			String src = GetHeaderField(field_name, _header.ToString());
			_err = Pop3DecodeHeaderField(_field, src, _header_size);
			if(_err < 0)
			{
				throw new nMailException("DecodeHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// メールヘッダから指定のヘッダフィールドの内容を取得し、
		/// MIME ヘッダフィールドのデコードを行って返します
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <param name="header">ヘッダ</param>
		/// <returns>取得したデコード済みのフィールド内容</returns>
		public string GetDecodeHeaderField(string field_name, string header)
		{
			SetHeaderSize();
			_err = Pop3DecodeHeaderField(_field, GetHeaderField(field_name, header), _header_size);
			if(_err < 0)
			{
				throw new nMailException("DecodeHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// 添付ファイル名の配列です。
		/// </summary>
		/// <returns>添付ファイル名の配列</returns>
		public string[] GetFileNameList()
		{
			if(_filename_list == null)
			{
				return new string[0];
			}
			else
			{
				return (string[])_filename_list.Clone();
			}
		}

		/// <summary>
		/// POP3 ポート番号です。
		/// </summary>
		/// <value>POP3 ポート番号</value>
		public int Port {
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}
		/// <summary>
		/// ソケットハンドルです。
		/// </summary>
		/// <value>ソケットハンドル</value>
		public IntPtr Handle {
			get
			{
				return _socket;
			}
		}
		/// <summary>
		/// POP3 サーバ上のメール数です。
		/// </summary>
		/// <value>POP3 サーバ上のメール数</value>
		public int Count {
			get
			{
				return _count;
			}
		}
		/// <summary>
		/// メールのサイズです。
		/// </summary>
		/// <value>メールのサイズ</value>
		public int Size {
			get
			{
				return _size;
			}
		}
		/// <summary>
		/// POP3 サーバ名です。
		/// </summary>
		/// <value>POP3 サーバ名</value>
		public string HostName
		{
			get
			{
				return _host;
			}
			set
			{
				_host = value;
			}
		}
		/// <summary>
		/// POP3 ユーザー名です。
		/// </summary>
		/// <value>POP3 ユーザー名</value>
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		/// <summary>
		/// POP3 パスワードです。
		/// </summary>
		/// <value>POP3 パスワード</value>
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
			}
		}
		/// <summary>
		/// 添付ファイルを保存するフォルダです。
		/// </summary>
		/// <remarks>
		/// null (VB.Net は nothing) の場合保存しません。
		/// </remarks>
		/// <value>添付ファイル保存フォルダ</value>
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}
		/// <summary>
		/// メールの本文です。
		/// </summary>
		/// <value>メール本文</value>
		public string Body
		{
			get
			{
				if(_body == null)
				{
					return "";
				}
				else
				{
					return _body.ToString();
				}
			}
		}
		/// <summary>
		/// メールの件名です。
		/// </summary>
		/// <value>メールの件名</value>
		public string Subject
		{
			get
			{
				if(_subject == null)
				{
					return "";
				}
				else
				{
					return _subject.ToString();
				}
			}
		}
		/// <summary>
		/// メールの送信日時の文字列です。
		/// </summary>
		/// <value>メール送信日時文字列</value>
		public string DateString
		{
			get
			{
				if(_date == null)
				{
					return "";
				}
				else
				{
					return _date.ToString();
				}
			}
		}
		/// <summary>
		/// メールの差出人です。
		/// </summary>
		/// <value>メールの差出人</value>
		public string From
		{
			get
			{
				if(_from == null)
				{
					return "";
				}
				else
				{
					return _from.ToString();
				}
			}
		}
		/// <summary>
		/// メールのヘッダです。
		/// </summary>
		/// <value>メールのヘッダ</value>
		public string Header
		{
			get
			{
				if(_header == null)
				{
					return "";
				}
				else
				{
					return _header.ToString();
				}
			}
			set
			{
				_header = new StringBuilder(value);
			}
		}
		/// <summary>
		/// 添付ファイル名です。
		/// </summary>
		/// <remarks>
		/// 複数の添付ファイルがある場合、"," で区切られて格納されます。
		/// <see cref="Options.SplitChar"/>で区切り文字を変更できます。
		/// </remarks>
		/// <value>添付ファイル名</value>
		public string FileName
		{
			get
			{
				if(_filename == null)
				{
					return "";
				}
				else
				{
					return _filename.ToString();
				}
			}
		}
		/// <summary>
		/// 添付ファイル名の配列です。
		/// </summary>
		/// <remarks>
		/// このプロパティは互換性のために残してあります。
		///	<see cref="GetFileNameList"/>で配列を取得して使用するようにしてください。
		/// </remarks>
		/// <value>添付ファイル名</value>
		public string[] FileNameList
		{
			get
			{
				return _filename_list;
			}
		}
		/// <summary>
		/// ヘッダのフィールド名です。
		/// </summary>
		/// <value>ヘッダのフィールド名</value>
		public string FieldName
		{
			get
			{
				if(_field_name == null)
				{
					return "";
				}
				else
				{
					return _field_name;
				}
			}
		}
		/// <summary>
		/// ヘッダフィールドの内容です。
		/// </summary>
		/// <value>ヘッダフィールドの内容</value>
		public string Field
		{
			get
			{
				if(_field == null)
				{
					return "";
				}
				else
				{
					return _field.ToString();
				}
			}
		}
		/// <summary>
		/// 添付ファイルの分割数です。
		/// </summary>
		/// <value>添付ファイルの分割数</value>
		public int PartNo
		{
			get
			{
				return _part_no;
			}
		}
		/// <summary>
		/// 添付ファイルの ID です。
		/// </summary>
		/// <value>添付ファイルの ID</value>
		public string PartId
		{
			get
			{
				if(_part_id == null)
				{
					return "";
				}
				else
				{
					return _part_id.ToString();
				}
			}
		}
		/// <summary>
		/// メールの UIDL です。
		/// </summary>
		/// <value>メール UIDL</value>
		public string Uidl
		{
			get
			{
				if(_uidl == null)
				{
					return "";
				}
				else
				{
					return _uidl.ToString();
				}
			}
		}
		/// <summary>
		/// APOP を使用するかどうかのフラグです。
		/// </summary>
		/// <remarks>
		/// true で APOP を使用します。
		/// </remarks>
		/// <value>APOP を使用するかどうかのフラグ</value>
		public bool APop
		{
			get
			{
				return _apop;
			}
			set
			{
				_apop = value;
			}
		}
		/// <summary>
		/// メール拡張機能指定フラグです。
		/// </summary>
		/// <remarks>
		/// <para><see cref="PartialAttachmentFile"/>分割された添付ファイルを取得します。</para>
		/// <para><see cref="TextOnly"/>メール本文のみ取得します。</para>
		/// <para><see cref="SuspendAttachmentFile"/>添付ファイル受信で一時休止ありの一回目に指定します。</para>
		/// <para><see cref="SuspendNext"/>添付ファイル受信で一時休止ありの二回目以降に指定します。</para>
		/// </remarks>
		public int Flag
		{
			get
			{
				return _flag;
			}
			set
			{
				_flag = value;
			}
		}
		/// <summary>
		/// エラー番号です。
		/// </summary>
		/// <value>エラー番号</value>
		public int ErrorCode
		{
			get
			{
				return _err;
			}
		}
		/// <summary>
		/// text/html パートを保存したファイルの名前です。
		/// </summary>
		/// <value>ファイル名</value>
		public string HtmlFile
		{
			get
			{
				if(_html_file == null)
				{
					return "";
				}
				else
				{
					return _html_file.ToString();
				}
			}
		}
		/// <summary>
		/// SSL 設定フラグです。
		/// </summary>
		/// <value>SSL 設定フラグ</value>
		public int SSL {
			get
			{
				return _ssl;
			}
			set
			{
				_ssl = value;
			}
		}
		/// <summary>
		/// SSL クライアント証明書名です。
		/// </summary>
		/// <remarks>
		/// null の場合指定しません。
		/// </remarks>
		/// <value>SSL クライアント証明書名</value>
		public string CertName
		{
			get
			{
				return _cert_name;
			}
			set
			{
				_cert_name = value;
			}
		}
		/// <summary>
		/// message/rfc822 パートを保存したファイルの名前です。
		/// </summary>
		/// <value>ファイル名</value>
		public string Rfc822File
		{
			get
			{
				if(_rfc822_file == null)
				{
					return "";
				}
				else
				{
					return _rfc822_file.ToString();
				}
			}
		}

	}

	/// <summary>
	/// SMTP メール送信クラス
	/// </summary>
	/// <example>
	/// <para>
	/// メールを送信する。
	///	<code lang="cs">
	///	using(nMail.Smtp smtp = new nMail.Smtp("mail.example.com")) {
	///		try {
	///			smtp.SendMail("to@example.net", "from@example.com", "テスト", "本文一行目\r\n二行目");
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim smtp As nMail.Smtp = New nMail.Smtp("mail.example.com")
	///	Try
	/// 	smtp.SendMail("to@example.net", "from@example.com", "テスト", "本文一行目" + ControlChars.CrLf + "二行目")
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		smtp.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// SMTP AUTH を使用してメールを送信する。
	///	<code lang="cs">
	///	using(nMail.Smtp smtp = new nMail.Smtp("mail.example.com")) {
	///		try {
	///			smtp.Connect();
	///			smpt.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain | nMail.Smtp.AuthCramMd5);
	///			smtp.SendMail("to@example.net", "from@example.com", "テスト", "本文一行目\r\n二行目");
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim smtp As nMail.Smtp = New nMail.Smtp("mail.example.com")
	///	Try
	/// 	smtp.Connect()
	/// 	smtp.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain Or nMail.Smtp.AuthCramMd5)
	/// 	smtp.SendMail("to@example.net", "from@example.com", "テスト", "本文一行目" + ControlChars.CrLf + "二行目")
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		smtp.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// TLS Version 1 と SMTP AUTH を使用してメールを送信する。
	///	<code lang="cs">
	///	using(nMail.Smtp smtp = new nMail.Smtp("mail.example.com")) {
	///		try {
	///			smtp.SSL = nMail.Smtp.TLS1;
	///			smtp.Connect(nMail.Smtp.StandardSslPortNo);		// over SSL/TLS のポート番号を指定して接続
	///			smpt.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain | nMail.Smtp.AuthCramMd5);
	///			smtp.SendMail("to@example.net", "from@example.com", "テスト", "本文一行目\r\n二行目");
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim smtp As nMail.Smtp = New nMail.Smtp("mail.example.com")
	///	Try
	/// 	smtp.SSL = nMail.Smtp.TLS1
	/// 	smtp.Connect(nMail.Smtp.StandardSslPortNo)		' over SSL/TLS のポート番号を指定して接続
	/// 	smtp.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain Or nMail.Smtp.AuthCramMd5)
	/// 	smtp.SendMail("to@example.net", "from@example.com", "テスト", "本文一行目" + ControlChars.CrLf + "二行目")
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		smtp.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// 一時休止機能を使ってメールを送信する。
	///	<code lang="cs">
	///	using(nMail.Smtp smtp = new nMail.Smtp("mail.example.com")) {
	///		try {
	///			// 添付ファイルの指定
	///			smtp.FileName = @"z:\file.dat";
	///			smtp.Flag = nMail.Smtp.SuspendAttachmentFile;
	///			smtp.Connect();
	///			// 一時休止機能のみ使う場合でも Authenticate の呼び出しは必要です。
	///			// Authenticate("", "", AuthNotUse); 等でも可。
	///			smtp.Authenticate();
	///			smtp.SendMail("to@example.net", "from@example.com", "テスト", "本文一行目\r\n二行目");
	///			smtp.Flag = nMail.Smtp.SuspendNext;
	///			while(smtp.ErrorCode == nMail.Smtp.ErrorSuspendAttachmentFile)
	///			{
	///				smtp.SendMail();
	///				// 休止中の処理を入れる
	///				Application.DoEvents();
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim smtp As nMail.Smtp = New nMail.Smtp("mail.example.com")
	///	Try
	///		smtp.FileName = "z:\file.dat"
	/// 	smtp.Flag = nMail.Smtp.SuspendAttachmentFile
	///		smtp.Connect()
	/// 	smtp.Authenticate();
	/// 	smtp.SendMail("to@example.net", "from@example.com", "テスト", "本文一行目" + ControlChars.CrLf + "二行目")
	///		smtp.Flag = nMail.Smtp.SuspendNext
	///		Do While smtp.ErrorCode = nMail.Smtp.ErrorSuspendAttachmentFile
	///			smtp.SendMail()
	///			' 休止中の処理を入れる
	///			Application.DoEvents()
	///		Loop
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		smtp.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// SMTP AUTH を使用し、さらに一時休止機能を使ってメールを送信する。
	///	<code lang="cs">
	///	using(nMail.Smtp smtp = new nMail.Smtp("mail.example.com")) {
	///		try {
	///			// 添付ファイルの指定
	///			smtp.FileName = @"z:\file.dat";
	///			smtp.Flag = nMail.Smtp.SuspendAttachmentFile;
	///			smtp.Connect();
	///			smpt.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain | nMail.Smtp.AuthCramMd5);
	///			smtp.SendMail("to@example.net", "from@example.com", "テスト", "本文一行目\r\n二行目");
	///			smtp.Flag = nMail.Smtp.SuspendNext;
	///			while(smtp.ErrorCode == nMail.Smtp.ErrorSuspendAttachmentFile)
	///			{
	///				smtp.SendMail();
	///				// 休止中の処理を入れる
	///				Application.DoEvents();
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim smtp As nMail.Smtp = New nMail.Smtp("mail.example.com")
	///	Try
	///		smtp.FileName = "z:\file.dat"
	/// 	smtp.Flag = nMail.Smtp.SuspendAttachmentFile
	///		smtp.Connect()
	///		smpt.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain Or nMail.Smtp.AuthCramMd5)
	/// 	smtp.SendMail("to@example.net", "from@example.com", "テスト", "本文一行目" + ControlChars.CrLf + "二行目")
	///		smtp.Flag = nMail.Smtp.SuspendNext
	///		Do While smtp.ErrorCode = nMail.Smtp.ErrorSuspendAttachmentFile
	///			smtp.SendMail()
	///			' 休止中の処理を入れる
	///			Application.DoEvents()
	///		Loop
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		smtp.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// </example>
	public class Smtp : IDisposable
	{
		/// <summary>
		/// 拡張機能用バッファのサイズです。
		/// </summary>
		protected const int AttachmentTempSize = 400;

		/// <summary>
		/// ソケットエラーもしくは非接続状態です。値は -1 です。
		/// </summary>
		public const int ErrorSocket = -1;
		/// <summary>
		/// 認証失敗エラーです。値は -2 です。
		/// </summary>
		public const int ErrorAuthenticate = -2;
		/// <summary>
		/// タイムアウトエラーです。値は -4 です。
		/// </summary>
		public const int ErrorTimeout = -4;
		/// <summary>
		/// 添付ファイルオープン失敗エラーです。値は -5 です。
		/// </summary>
		public const int ErrorFileOpen = -5;
		/// <summary>
		/// サーバが指定した認証形式に対応していません。値は -8 です。
		/// </summary>
		public const int ErrorAuthenticateNoSupport = -8;
		/// <summary>
		/// メモリ確保エラーです。値は -9 です。
		/// </summary>
		public const int ErrorMemory = -9;
		/// <summary>
		/// その他のエラーです。値は -10 です。
		/// </summary>
		public const int ErrorEtc = -10;
		/// <summary>
		/// メール送信中の一時休止状態です。値は -20 です。
		/// </summary>
		public const int ErrorSuspendAttachmentFile = -20;
		/// <summary>
		/// サービスが利用できません。値は -421 です。
		/// </summary>
		public const int ErrorServiceNotAvaliable = -421;
		/// <summary>
		/// メールボックスが使用できません。（使用中等）値は -450 です。
		/// </summary>
		public const int ErrorMailboxUnavalable = -450;
		/// <summary>
		/// サーバ処理エラーです。値は -451 です。
		/// </summary>
		public const int ErrorLocal = -451;
		/// <summary>
		/// システム容量不足エラーです。値は -452 です。
		/// </summary>
		public const int ErrorInsufficientSystemStorage = -452;
		/// <summary>
		/// コマンドの文法エラーです。値は -500 です。
		/// </summary>
		public const int ErrorSyntax = -500;
		/// <summary>
		/// パラメータか引数の文法エラーです。値は -501 です。
		/// </summary>
		public const int ErrorParameter = -501;
		/// <summary>
		/// 未実装コマンドです。値は -502 です。
		/// </summary>
		public const int ErrorCommandNotImplemented = -502;
		/// <summary>
		/// コマンドのシーケンス異常です。値は -503 です。
		/// </summary>
		public const int ErrorBadSequence =-503;
		/// <summary>
		/// SMTP 認証失敗です。値は -535 です。
		/// </summary>
		public const int ErrorSmtpAuthenticate = -535;
		/// <summary>
		/// 送信先がありません。値は -550 です。
		/// </summary>
		public const int ErrorUserUnkown = -550;
		/// <summary>
		/// ユーザーが存在しないか、転送先がありません。値は -551 です。
		/// </summary>
		public const int ErrorUserNotLocal = -551;
		/// <summary>
		/// 容量オーバーエラーです。値は -552 です。
		/// </summary>
		public const int ErrorExceededStorageAllocation =-552;
		/// <summary>
		/// メールボックス名が正しくありません。値は -553 です。
		/// </summary>
		public const int ErrorMailboxNameNotAllowed = -553;
		/// <summary>
		/// リレー防止エラーです。値は -553 です。
		/// </summary>
		public const int ErrorRelayOperationRejected =-553;
		/// <summary>
		/// トランザクションに失敗しました。値は -554 です。
		/// </summary>
		public const int ErrorTransactionFailed = -554;
		/// <summary>
		/// 送信日時フィールドを nMail.DLL 内で生成します。
		/// </summary>
		public const int AddDateField = 1;
		/// <summary>
		/// Message-ID フィールドを nMail.DLL 内で生成します。
		/// </summary>
		public const int AddMessageId = 2;
		/// <summary>
		/// 大きなサイズの添付ファイル送信中に一旦処理を戻します。
		/// </summary>
		public const int SuspendAttachmentFile = 4;
		/// <summary>
		/// 大きなサイズの添付ファイル送信二回目以降です。
		/// </summary>
		public const int SuspendNext = 8;
		/// <summary>
		/// RFC2231 で添付ファイル名をエンコードします。
		/// </summary>
		public const int FileNameRfc2231 = 16;
		/// <summary>
		/// UTF-8 でメールをエンコードします（さらにBASE64でエンコードされます）
		/// </summary>
		public const int EncodeUtf8 = 32;
		/// <summary>
		/// HTML メールを送信します。
		/// </summary>
		public const int HtmlMail = 64;
		/// <summary>
		/// text/html パートをファイルで指定します。
		/// </summary>
		public const int HtmlUseFile = 128;
		/// <summary>
		/// HTML メールの宛先のドメインをみて multipart の構成を決定します。
		/// </summary>
		public const int HtmlAutoPart = 256;
		/// <summary>
		/// text/html パートで使用する画像ファイルを添付する場合、multipart/mixed なしで送信します。
		/// </summary>
		public const int HtmlNoMixedPart = 512;
		/// <summary>
		/// Softbank 携帯向けオプション（HtmlNoMixedPartと同じ）
		/// </summary>
		public const int HtmlSoftbank = 512;
		/// <summary>
		/// EMOBILE 携帯向けオプション（HtmlNoMixedPartと同じ）
		/// </summary>
		public const int HtmlEmobile = 512;
		/// <summary>
		/// WILLCOM 携帯向けオプション（HtmlNoMixedPartと同じ）
		/// </summary>
		public const int HtmlWillcom = 512;
		/// <summary>
		/// text/html パートで使用する画像ファイルを添付する場合、multipart/related なしで送信します。
		/// </summary>
		public const int HtmlNoRelatedPart = 1024;
		/// <summary>
		/// au 携帯向けオプション（HtmlNoRelatedPartと同じ）
		/// </summary>
		public const int HtmlAu = 1024;
		/// <summary>
		/// text/html パートのみ（text/plain パートなし）で HTML メールを送信します。
		/// </summary>
		public const int HtmlNoPlainPart = 2048;
		/// <summary>
		/// text/plain パートで使用する文字列を Body で指定します。
		/// </summary>
		public const int HtmlSetPlainBody = 4096;
		/// <summary>
		/// 処理成功です。
		/// </summary>
		public const int Success = 1;
		/// <summary>
		/// SMTP AUTH は使用しません。
		/// </summary>
		public const int AuthNotUse = 0;
		/// <summary>
		/// SMTP AUTH PLAIN を使用します。
		/// </summary>
		public const int AuthPlain = 1;
		/// <summary>
		/// SMTP AUTH LOGIN を使用します。
		/// </summary>
		public const int AuthLogin = 2;
		/// <summary>
		/// SMTP AUTH CRAM MD5 を使用します。
		/// </summary>
		public const int AuthCramMd5 = 4;
		/// <summary>
		/// SMTP AUTH DIGEST MD5 を使用します。
		/// </summary>
		public const int AuthDigestMd5 = 8;
		/// <summary>
		/// SSLv3 を使用します。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int SSL3 = 0x1000;
		/// <summary>
		/// TSLv1 を使用します。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int TLS1 = 0x2000;
		/// <summary>
		/// STARTTLS を使用します。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int STARTTLS = 0x4000;
		/// <summary>
		/// サーバ証明書が期限切れでもエラーにしません。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int IgnoreNotTimeValid = 0x0800;
		/// <summary>
		/// ルート証明書が無効でもエラーにしません。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int AllowUnknownCA = 0x0400;
		/// <summary>
		/// Common Name が一致しなくてもエラーにしません。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int IgnoreInvalidName = 0x0040;
		/// <summary>
		/// SMTP の標準ポート番号です
		/// </summary>
		public const int StandardPortNo = 25;
		/// <summary>
		/// SMTP のサブミッションポート番号です
		/// </summary>
		public const int SubmissionPortNo = 587;
		/// <summary>
		/// SMTP over SSL のポート番号です
		/// </summary>
		public const int StandardSslPortNo = 465;

		/// <summary>
		/// SMTP ポート番号です。
		/// </summary>
		protected int _port = 25;
		/// <summary>
		/// ソケットハンドルです。
		/// </summary>
		protected IntPtr _socket = (IntPtr)ErrorSocket;
		/// <summary>
		/// メール送信拡張機能の指定です。
		/// </summary>
		protected int _flag = 0;
		/// <summary>
		/// エラー番号です。
		/// </summary>
		protected int _err = 0;
		/// <summary>
		/// SMTP AUTH の認証方法です。
		/// </summary>
		protected int _mode = AuthNotUse;
		/// <summary>
		/// SMTP サーバ名です。
		/// </summary>
		protected string _host = "";
		/// <summary>
		/// SMTP AUTH ユーザー名です。
		/// </summary>
		protected string _id = "";
		/// <summary>
		/// SMTP AUTH パスワードです。
		/// </summary>
		protected string _password = "";
		/// <summary>
		/// メールの宛先です。
		/// </summary>
		protected string _to = "";
		/// <summary>
		/// メールの CC です。
		/// </summary>
		protected string _cc = "";
		/// <summary>
		/// メールの BCC です。
		/// </summary>
		protected string _bcc = "";
		/// <summary>
		/// メールの差出人です。
		/// </summary>
		protected string _from = "";
		/// <summary>
		/// メールの件名です。
		/// </summary>
		protected string _subject = "";
		/// <summary>
		/// メールの本文です。
		/// </summary>
		protected string _body = "";
		/// <summary>
		/// メールの追加ヘッダです。
		/// </summary>
		protected string _header = "";
		/// <summary>
		/// メールに添付するファイル名です。
		/// </summary>
		protected string _filename = "";
		/// <summary>
		/// 添付ファイル名のリストです。
		/// </summary>
		protected string[] _filename_list = null;
		/// <summary>
		/// 拡張機能用バッファです。
		/// </summary>
		protected byte[] _temp = null;
		/// <summary>
		/// Connect() を使ったかどうかのフラグです。
		/// </summary>
		protected bool _connect_flag = false;
		/// <summary>
		/// Dispose 処理を行ったかどうかのフラグです。
		/// </summary>
		private bool _disposed = false;
		/// <summary>
		/// SSL 設定フラグです。
		/// </summary>
		protected int _ssl = 0;
		/// <summary>
		/// SSL クライアント証明書名です。
		/// </summary>
		protected string _cert_name = null;

		[DllImport("nMail.DLL", EntryPoint="NMailSmtpSendMailPortNo", CharSet=CharSet.Auto)]
		protected static extern int SmtpSendMailPortNo(String HostName, String To, String Cc, String Bcc, String From, String Subject, String Body, String Header, String Path, int Flag, int PortNo);

		[DllImport("nMail.DLL", EntryPoint="NMailSmtpConnect", CharSet=CharSet.Auto)]
		protected static extern IntPtr SmtpConnect(string Host);

		[DllImport("nMail.DLL", EntryPoint="NMailSmtpConnectPortNo", CharSet=CharSet.Auto)]
		protected static extern IntPtr SmtpConnectPortNo(string Host, int PortNo);

		[DllImport("nMail.DLL", EntryPoint="NMailSmtpConnectSsl", CharSet=CharSet.Auto)]
		protected static extern IntPtr SmtpConnectSsl(string Host, int Port, int Flag, string Name);

		[DllImport("nMail.DLL", EntryPoint="NMailSmtpSendMailSsl", CharSet=CharSet.Auto)]
		protected static extern int SmtpSendMailSsl(String HostName, String To, String Cc, String Bcc, String From, String Subject, String Body, String Header, String Path, int Flag, int PortNo, int SslFlag, String Name);

		[DllImport("nMail.DLL", EntryPoint="NMailSmtpAuthenticate", CharSet=CharSet.Auto)]
		protected static extern int SmtpAuthenticate(IntPtr Socket, string HostName, string Id, string Pass, int Mode);

		[DllImport("nMail.DLL", EntryPoint="NMailSmtpSendMailEx", CharSet=CharSet.Auto)]
		protected static extern int SmtpSendMailEx(IntPtr Socket, string To, string Cc, string Bcc, string From, string Subject, String Body, String Header, String Path, byte[] Temp, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailSmtpClose")]
		protected static extern int SmtpClose(IntPtr Socket);

		/// <summary>
		/// <c>Smtp</c>クラスの新規インスタンスを初期化します。
		/// </summary>
		public Smtp()
		{
			Init();
		}
		/// <summary>
		/// <c>Smtp</c>クラスの新規インスタンスを初期化します。
		/// </summary>
		/// <param name="host_name">SMTP サーバ名</param>
		public Smtp(string host_name)
		{
			_host = host_name;
			Init();
		}
		/// <summary>
		/// 
		/// </summary>
		~Smtp()
		{
			Dispose(false);
		}
		/// <summary>
		/// <see cref="Smtp"/>によって使用されているすべてのリソースを解放します。
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// 初期化処理です。
		/// </summary>
		protected void Init()
		{
			_temp = new byte[AttachmentTempSize];
		}
		/// <summary>
		/// <see cref="Smtp"/>によって使用されているすべてのリソースを解放します。
		/// </summary>
		/// <param name="disposing">
		/// マネージリソースとアンマネージリソースの両方を解放する場合は<c>true</c>。
		/// アンマネージリソースだけを解放する場合は<c>false</c>。
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			if(!_disposed) {
				if(disposing)
				{
				}
				if(_connect_flag && _socket != (IntPtr)ErrorSocket) {
					SmtpClose(_socket);
				}
				_connect_flag = false;
				_socket = (IntPtr)ErrorSocket;
				_disposed = true;
			}
		}
		/// <summary>
		/// メールを送信します。
		/// </summary>
		/// <remarks>
		///	一時休止機能の二回目以降の呼び出しで使用します。
		/// </remarks>
		/// <exception cref="FileNotFoundException">
		/// <see cref="FileName"/>で設定されているファイルが存在しません。
		/// </exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void SendMail()
		{
			_filename = "";
			if(_filename_list != null)
			{
				foreach(string name in _filename_list)
				{
					if(!File.Exists(name))
					{
						throw new FileNotFoundException(name);
					}
					_filename = String.Join(Convert.ToString(Options.SplitChar), _filename_list);
				}
			}
			if(_connect_flag)
			{
				_err = SmtpSendMailEx(_socket, _to, _cc, _bcc, _from, _subject, _body, _header, _filename, _temp, _flag);
			}
			else
			{
				_err = SmtpSendMailSsl(_host, _to, _cc, _bcc, _from, _subject, _body, _header, _filename, _flag, _port, _ssl, _cert_name);
			}
			if(_err < 0 && _err != ErrorSuspendAttachmentFile)
			{
				throw new nMailException("SendMail: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// メールを送信します。
		/// </summary>
		/// <param name="to_str">宛先</param>
		/// <param name="from_str">差出人</param>
		/// <param name="subject_str">件名</param>
		/// <param name="body_str">本文</param>
		/// <remarks>
		/// <paramref name="to_str"/>パラメータでメールの宛先を指定します。
		/// <paramref name="from_str"/>パラメータで差出人を指定します。
		/// <paramref name="subject_str"/>パラメータで件名を指定します。
		/// <paramref name="body_str"/>パラメータで本文を指定します。
		/// <para>CC を指定したい場合、<see cref="Cc"/>に指定しておきます。</para>
		/// <para>BCC を指定したい場合、<see cref="Bcc"/>に指定しておきます。</para>
		/// <para>ファイルを添付したい場合、<see cref="FileName"/>に指定しておきます。</para>
		/// </remarks>
		/// <exception cref="FileNotFoundException">
		/// <see cref="FileName"/>で設定されているファイルが存在しません。
		/// </exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void SendMail(string to_str, string from_str, string subject_str, string body_str)
		{
			_to = to_str;
			_from = from_str;
			_subject = subject_str;
			_body = body_str;
			SendMail();
		}
		/// <summary>
		/// SMTP サーバに接続します。
		/// </summary>
		/// <remarks>
		///	SMTP AUTH および一時休止機能を使う場合に使用します。
		/// </remarks>
		///	<exception cref="FormatException">
		///	サーバ名に文字列が入っていません。
		/// </exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect()
		{
			if(_host == "") {
				throw new FormatException();
			}
			if(_port < 0 || _port > 65536) {
				throw new ArgumentOutOfRangeException();
			}
			//_socket = SmtpConnectPortNo(_host, _port);
			_socket = SmtpConnectSsl(_host, _port, _ssl, _cert_name);
			if(_socket == (IntPtr)ErrorSocket)
			{
				_err = ErrorSocket;
				_socket = (IntPtr)ErrorSocket;
				throw new nMailException("Connect", _err);
			} else {
				_connect_flag = true;
			}
		}
		/// <summary>
		/// SMTP サーバに接続
		/// </summary>
		/// <param name="host_name">SMTP サーバ名</param>
		/// <remarks>
		///	SMTP AUTH および一時休止機能を使う場合に使用します。
		/// </remarks>
		///	<exception cref="FormatException">
		///	サーバ名に文字列が入っていません。
		/// </exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect(string host_name)
		{
			_host = host_name;
			Connect();
		}
		/// <summary>
		/// SMTP サーバに接続
		/// </summary>
		/// <param name="host_name">SMTP サーバ名</param>
		/// <param name="port_no">ポート番号</param>
		/// <remarks>
		///	SMTP AUTH および一時休止機能を使う場合に使用します。
		/// </remarks>
		///	<exception cref="FormatException">
		///	サーバ名に文字列が入っていません。
		/// </exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect(string host_name, int port_no)
		{
			_host = host_name;
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// SMTP サーバに接続
		/// </summary>
		/// <param name="port_no">ポート番号</param>
		/// <remarks>
		///	SMTP AUTH および一時休止機能を使う場合に使用します。
		/// </remarks>
		///	<exception cref="FormatException">
		///	サーバ名に文字列が入っていません。
		/// </exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect(int port_no)
		{
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// SMTP サーバとの接続終了
		/// </summary>
		public void Close()
		{
			if(_socket != (IntPtr)ErrorSocket) {
				SmtpClose(_socket);
			}
			_socket = (IntPtr)ErrorSocket;
			_connect_flag = false;
		}
		/// <summary>
		/// SMTP AUTH 認証
		/// </summary>
		///	<remarks>
		///	一時休止機能を使う場合に<see cref="Connect"/>の後に呼び出します。
		///	</remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。<see cref="Connect"/>が成功していないか、呼び出していません。このメソッドを呼び出す場合は(<see cref="Connect"/>を呼び出す必要があります。。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Authenticate()
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = SmtpAuthenticate(_socket, _host, _id, _password, _mode);
			if(_err < 0)
			{
				throw new nMailException("Authenticate: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// SMTP AUTH 認証
		/// </summary>
		/// <param name="id_str">ユーザー ID</param>
		/// <param name="pass_str">パスワード</param>
		/// <param name="mode">認証形式</param>
		/// <para>認証形式の設定可能な値は下記の通りです。</para>
		/// <para><see cref="AuthPlain"/> で PLAIN を使用します。</para>
		/// <para><see cref="AuthLogin"/> で LOGIN を使用します。</para>
		/// <para><see cref="AuthCramMd5"/> CRAM MD5 を使用します。</para>
		/// <para>AuthNotUse 以外は C# は | 、VB.NET では Or で複数指定可能です。CRAM MD5→LOGIN→PLAIN の優先順位で認証を試みます。</para>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。<see cref="Connect"/>が成功していないか、呼び出していません。このメソッドを呼び出す場合は(<see cref="Connect"/>を呼び出す必要があります。。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Authenticate(string id_str, string pass_str, int mode)
		{
			_id = id_str;
			_password = pass_str;
			_mode = mode;
			Authenticate();
		}
		/// <summary>
		/// SMTP ポート番号です。
		/// </summary>
		/// <value>SMTP ポート番号</value>
		public int Port {
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}
		/// <summary>
		/// ソケットハンドルです。
		/// </summary>
		/// <value>SMTP ソケットハンドル</value>
		public IntPtr Handle {
			get
			{
				return _socket;
			}
		}
		/// <summary>
		/// SMTP サーバ名です。
		/// </summary>
		/// <value>SMTP サーバ名</value>
		public string HostName
		{
			get
			{
				return _host;
			}
			set
			{
				_host = value;
			}
		}
		/// <summary>
		/// SMTP AUTH 用のユーザー名です。
		/// </summary>
		/// <value>SMTP AUTH ユーザー名</value>
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		/// <summary>
		/// SMTP AUTH 用のパスワードです。
		/// </summary>
		/// <value>SMTP AUTH パスワード</value>
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
			}
		}
		/// <summary>
		/// メールの宛先です。
		/// </summary>
		/// <remarks>
		/// 複数の宛先に送信する場合、 ',' で区切ってメールアドレスを記述してください。
		/// </remarks>
		/// <value>メール 宛先</value>
		public string MailTo
		{
			get
			{
				return _to;
			}
			set
			{
				_to = value;
			}
		}
		/// <summary>
		/// メールの CC です。
		/// </summary>
		/// <remarks>
		/// 複数の宛先に送信する場合、 ',' で区切ってメールアドレスを記述してください。
		/// </remarks>
		/// <value>メール CC</value>
		public string Cc
		{
			get
			{
				return _cc;
			}
			set
			{
				_cc = value;
			}
		}
		/// <summary>
		/// メールの BCC です。
		/// </summary>
		/// <remarks>
		/// 複数の宛先に送信する場合、 ',' で区切ってメールアドレスを記述してください。
		/// </remarks>
		/// <value>メール BCC</value>
		public string Bcc
		{
			get
			{
				return _bcc;
			}
			set
			{
				_bcc = value;
			}
		}
		/// <summary>
		/// メールの差出人です。
		/// </summary>
		/// <value>メール差出人</value>
		public string From
		{
			get
			{
				return _from;
			}
			set
			{
				_from = value;
			}
		}
		/// <summary>
		/// メールの件名です。
		/// </summary>
		/// <value>メール件名</value>
		public string Subject
		{
			get
			{
				return _subject;
			}
			set
			{
				_subject = value;
			}
		}
		/// <summary>
		/// メールの本文です。
		/// </summary>
		/// <value>メール本文</value>
		public string Body
		{
			get
			{
				return _body;
			}
			set
			{
				_body = value;
			}
		}
		/// <summary>
		/// メールの添付ファイルです。
		/// </summary>
		/// <remarks>
		/// 複数の添付ファイルを送信する場合、"," で区切ります。
		/// <see cref="Options.SplitChar"/>で区切り文字を変更できます。
		/// </remarks>
		/// <value>添付ファイル</value>
		public string FileName
		{
			get
			{
				_filename = String.Join(Convert.ToString(Options.SplitChar), _filename_list);
				return _filename;
			}
			set
			{
				_filename = value;
				if(_filename.Length > 0) 
				{
					_filename_list = _filename.ToString().Split(Options.SplitChar);
				} 
				else 
				{
					_filename_list = null;
				}
			}
		}
		/// <summary>
		/// 添付ファイル名の配列です。
		/// </summary>
		/// <value>添付ファイル名の配列</value>
		public string[] FileNameList
		{
			get
			{
				return _filename_list;
			}
			set
			{
				_filename_list = value;
			}
		}
		/// <summary>
		/// メールの追加ヘッダです。
		/// </summary>
		/// <remarks>
		/// 複数のヘッダを追加する場合、C# では '\r\n'、Visual Basic では ControlChars.CrLf
		/// で連結してください。
		/// </remarks>
		/// <value>追加ヘッダ</value>
		public string Header
		{
			get
			{
				return _header;
			}
			set
			{
				_header = value;
			}
		}
		/// <summary>
		/// メール送信のオプション指定です。
		/// </summary>
		/// <value>送信オプション</value>
		/// <remarks>
		/// <para><see cref="AddDateField"/>送信日時フィールドを nMail.DLL 内で生成します。</para>
		/// <para><see cref="AddMessageId"/>Message-ID フィールドを nMail.DLL 内で生成します。</para>
		/// <para><see cref="SuspendAttachmentFile"/>添付ファイル送信で一時休止ありの一回目に指定します。</para>
		/// <para><see cref="SuspendNext"/>添付ファイル送信で一時休止ありの二回目以降に指定します。</para>
		///	<para><see cref="FileNameRfc2231"/>RFC2231 で添付ファイル名をエンコードします。</para>
		/// <para><see cref="EncodeUtf8"/>UTF-8 でメールをエンコードします（さらにBASE64でエンコードされます）</para>
		/// <para>C# の場合 |、VB.NET の場合 Or で複数の指定が可能です。ただし SuspendAttachmentFile と SuspendNext は同時に指定できません。</para>
		/// </remarks>
		public int Flag
		{
			get
			{
				return _flag;
			}
			set
			{
				_flag = value;
			}
		}
		/// <summary>
		/// SMTP AUTH の種別です。
		/// </summary>
		/// <remarks>
		/// <see cref="AuthNotUse"/> で SMTP AUTH は使用しません。
		/// <see cref="AuthPlain"/> で PLAIN を使用します。
		/// <see cref="AuthLogin"/> で LOGIN を使用します。
		/// <see cref="AuthCramMd5"/> CRAM MD5 を使用します。
		/// <see cref="AuthDigestMd5"/> DIGEST MD5 を使用します。
		/// AuthNotUse 以外は C# は | 、Visual Basic では or で複数設定可能です。
		/// DIGEST MD5→CRAM MD5→LOGIN→PLAIN の順番で認証を試みます。
		///	デフォルト値は AuthNotUse です。
		/// </remarks>
		/// <value>送信モード</value>
		public int AuthMode
		{
			get
			{
				return _mode;
			}
			set
			{
				_mode = value;
			}
		}
		/// <summary>
		/// メール送信時のエラー番号です。
		/// </summary>
		/// <value>エラー番号</value>
		public int ErrorCode
		{
			get
			{
				return _err;
			}
		}
		/// <summary>
		/// SSL 設定フラグです。
		/// </summary>
		/// <value>SSL 設定フラグ</value>
		public int SSL {
			get
			{
				return _ssl;
			}
			set
			{
				_ssl = value;
			}
		}
		/// <summary>
		/// SSL クライアント証明書名です。
		/// </summary>
		/// <remarks>
		/// null の場合指定しません。
		/// </remarks>
		/// <value>SSL クライアント証明書名</value>
		public string CertName
		{
			get
			{
				return _cert_name;
			}
			set
			{
				_cert_name = value;
			}
		}

	}
	/// <summary>
	/// 添付ファイル保存クラス
	/// </summary>
	/// <example>
	/// <para>
	///	分割されたメールデータを読み出し、結合して保存する。
	/// text/html パートをファイルに保存する場合、<see cref="Options.DisableDecodeBodyText()"/> もしくは <see cref="Options.DisableDecodeBodyAll()"/> を呼んだ後に Pop3.GetMail() を呼び出して
	/// 取得したヘッダおよび本文データを使用し、<see cref="Attachment.Save()"/> の前に <see cref="Options.EnableDecodeBody()"/> と <see cref="Options.EnableSaveHtmlFile()"/> を呼んでおく必要があります。
	/// <code lang="cs">
	///	using(nMail.Attachment attach = new nMail.Attachment()) {
	///		try {
	///			attach.Path = @"z:\temp";
	///			// count は分割数
	///			for(int no = 0 ; no &lt; count ; no++) {
	///				// Read() は header にヘッダ＋"\r\n"＋"本文"を読み出す処理
	///				string header = Read(no)
	///				attach.Add(header);
	///			}
	///			attach.Save();
	///			// 正常終了
	///			MessageBox.Show(attach.Path + " に添付ファイル " + attach.FileName + " を保存しました。\r\n\r\n件名:" + attach.Subject + "\r\n本文:\r\n" + attach.Body);
	///		}
	///		catch(nMailException nex) {
	///				if(nex.ErrorCode == nMail.Attachment.ErrorFileOpen) {
	///					MessageBox.Show("添付ファイルがオープンできません。");
	///				} else if(nex.ErrorCode == nMail.Attachment.ErrorInvalidNo) {
	///	                MessageBox.Show("分割されたメールの順番が正しくないか、該当しないファイルが入っています。")
	///				} else if(nex.ErrorCode == nMail.Attachment.ErrorPartial) {
	///					MessageBox.Show("分割されたメールが全て揃っていません");
	///				}
	///			} catch(Exception ex) {
	///				MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///			}
	///		}
	///	}
	/// </code>
	/// </para>
	/// <para>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	///	Dim attach As nMail.Attachment = New nMail.Attachment
	///	Try
	///		Dim no As Integer
	///		attach.Path = "z:\temp"
	///		// count は分割数
	///		For no = 0 To count - 1
	///			' Read() は header にヘッダ＋ControlChars.CrLf＋"本文"を読み出す処理
	///			string header = Read(no)
	///			attach.Add(header)
	///		Next no
	///		attach.Save()
	///		' 正常終了
	///		MessageBox.Show(attach.Path + " に添付ファイル " + attach.FileName + " を保存しました。" + ControlChars.CrLf + ControlChars.CrLf + "件名:" + attach.Subject + ControlChars.CrLf + "本文:" + ControlChars.CrLf + attach.Body)
	///	Catch nex As nMail.nMailException
	///		If nex.ErrorCode = nMail.Attachment.ErrorFileOpen Then
	///			MessageBox.Show("添付ファイルがオープンできません。")
	///		ElseIf nex.ErrorCode = nMail.Attachment.ErrorInvalidNo Then
	///			MessageBox.Show("分割されたメールの順番が正しくないか、該当しないファイルが入っています。")
	///		ElseIf nex.ErrorCode = nMail.Attachment.ErrorPartial Then
	///			MessageBox.Show("分割されたメールが全て揃っていません")
	///		End If
	///	Catch ex As Exception
	///		Debug.WriteLine(ex.Message)
	///	Finally
	///		attach.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// </example>
	public class Attachment : IDisposable
	{
		/// <summary>
		/// ヘッダ領域のサイズです。
		/// </summary>
		protected const int HeaderSize = 32768;
		/// <summary>
		/// パス文字列のサイズです。
		/// </summary>
		protected const int MaxPath = 32768;
		/// <summary>
		/// 添付ファイル展開用バッファのサイズです。
		/// </summary>
		protected const int AttachmentTempSize = 400;

		/// <summary>
		/// 分割された添付ファイルの順番が正しくありません。値は -3 です。
		/// </summary>
		public const int ErrorInvalidNo = -3;
		/// <summary>
		/// 添付ファイルを作成できませんでした。値は -5 です。
		/// </summary>
		public const int ErrorFileOpen = -5;
		/// <summary>
		/// 添付ファイルの展開が終了していません。値は -6 です。
		/// </summary>
		public const int ErrorPartial = -6;
		/// <summary>
		/// 添付ファイルと同名のファイルがフォルダに存在します。値は -7 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="nMail.Options.AlreadyFile"/> に <see cref="nMail.Options.FileAlreadyError"/></para>
		/// <para>を設定し、<see cref="Path"/>で指定したフォルダに添付ファイルと同じ名前のファイルが</para>
		/// <para>ある場合にこのエラーが発生します。</para>
		/// </remarks>
		public const int ErrorFileAlready = -7;
		/// <summary>
		/// メモリ確保エラーです。値は -9 です。
		/// </summary>
		public const int ErrorMemory = -9;
		/// <summary>
		/// その他のエラーです。値は -10 です。
		/// </summary>
		public const int ErrorEtc = -10;
		/// <summary>
		/// 添付ファイルは存在しません。
		/// </summary>
		public const int NoAttachmentFile = -1;
		/// <summary>
		/// ヘッダデータのリスト
		/// </summary>
		protected ArrayList _header_list;
		/// <summary>
		/// 本文データのリスト
		/// </summary>
		protected ArrayList _body_list;

		/// <summary>
		/// ヘッダーサイズです。
		/// </summary>
		protected int _header_size = -1;
		/// <summary>
		/// エラー番号です。
		/// </summary>
		protected int _err;
		/// <summary>
		/// 添付ファイルを保存するパスです。
		/// </summary>
		protected string _path = "";
		/// <summary>
		/// メールの件名です。
		/// </summary>
		protected StringBuilder _subject;
		/// <summary>
		/// メールの送信日時です。
		/// </summary>
		protected StringBuilder _date;
		/// <summary>
		/// メールの差出人です。
		/// </summary>
		protected StringBuilder _from;
		/// <summary>
		/// メールのヘッダです。
		/// </summary>
		protected StringBuilder _header;
		/// <summary>
		/// メールの本文です。
		/// </summary>
		protected StringBuilder _body;
		/// <summary>
		/// 展開した添付ファイル名です。
		/// </summary>
		protected StringBuilder _filename;
		/// <summary>
		/// 添付ファイル名のリストです。
		/// </summary>
		protected string[] _filename_list = null;
		/// <summary>
		/// 分割ファイルの ID です。
		/// </summary>
		protected StringBuilder _id;
		/// <summary>
		/// 添付ファイル展開用バッファです。
		/// </summary>
		protected byte[] _temp;
		/// <summary>
		/// 展開を開始したかどうかのフラグです。
		/// </summary>
		protected bool _attachment_flag = false;
		/// <summary>
		/// Dispose 処理を行ったかどうかのフラグです。
		/// </summary>
		private bool _disposed = false;
		/// <summary>
		/// text/html パートを保存したファイルの名前です。
		/// </summary>
		protected StringBuilder _html_file = null;
		/// <summary>
		/// ヘッダフィールド名です。
		/// </summary>
		protected string _field_name = "";
		/// <summary>
		/// ヘッダフィールド内容格納バッファです。
		/// </summary>
		protected StringBuilder _field = null;
		/// <summary>
		/// message/rfc822 パートを保存したファイルの名前です。
		/// </summary>
		protected StringBuilder _rfc822_file = null;

		[DllImport("nMail.DLL", EntryPoint="NMailAttachmentFileFirst", CharSet=CharSet.Auto)]
		protected static extern int AttachmentFileFirst(byte []Temp, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, StringBuilder Body, string Path, StringBuilder FileName, string FirstHeader, string FirstBody);

		[DllImport("nMail.DLL", EntryPoint="NMailAttachmentFileNextVB", CharSet=CharSet.Auto)]
		protected static extern int AttachmentFileNextVB(byte []Temp, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, StringBuilder Body, string Path, StringBuilder FileName, string NextHeader, string NextBody);

		[DllImport("nMail.DLL", EntryPoint="NMailAttachmentFileClose")]
		protected static extern bool AttachmentFileClose(byte []Temp);

		[DllImport("nMail.DLL", EntryPoint="NMailAttachmentFileStatus", CharSet=CharSet.Auto)]
		protected static extern int AttachmentFileStatus(string Header, StringBuilder Id, int Size);

		[DllImport("nMail.DLL", EntryPoint="NMailGetHeaderField", CharSet=CharSet.Auto)]
		protected static extern int Pop3GetHeaderField(StringBuilder Field, string Header, string Name, int Size);

		[DllImport("nMail.DLL", EntryPoint="NMailDecodeHeaderField", CharSet=CharSet.Auto)]
		protected static extern int Pop3DecodeHeaderField(StringBuilder Destination, string Source, int Size);

		/// <summary>
		/// <c>Attachment</c>クラスの新規インスタンスを初期化します。
		/// </summary>
		public Attachment()
		{
			_header_list = new ArrayList();
			_body_list = new ArrayList();
			_temp = new byte[AttachmentTempSize];
		}
		/// <summary>
		/// 
		/// </summary>
		~Attachment()
		{
			Dispose(false);
		}
		/// <summary>
		/// <see cref="Attachment"/>によって使用されているすべてのリソースを解放します。
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// <see cref="Attachment"/>によって使用されているすべてのリソースを解放します。
		/// </summary>
		/// <param name="disposing">
		/// マネージリソースとアンマネージリソースの両方を解放する場合は<c>true</c>。
		/// アンマネージリソースだけを解放する場合は<c>false</c>。
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			if(!_disposed)
			{
				if(disposing)
				{
				}
				if(_attachment_flag)
				{
					AttachmentFileClose(_temp);
					_attachment_flag = false;
				}
				_disposed = true;
			}
		}
		/// <summary>
		/// 添付ファイル展開で使用していたリソースを解放します。
		/// </summary>
		public void Close()
		{
			if(_attachment_flag)
			{
				AttachmentFileClose(_temp);
				_attachment_flag = false;
			}
		}
		/// <summary>
		/// ヘッダ格納用バッファのサイズを決定します。
		/// </summary>
		protected void SetHeaderSize()
		{
			if(_header_size < 0)
			{
				_header_size = Options.HeaderMax;
			}
			if(_header_size <= 0)
			{
				_header_size = HeaderSize;
				Options.HeaderMax = _header_size;
			}
		}
		/// <summary>
		/// メール本文の合計サイズを取得します。
		/// </summary>
		/// <returns>メール本文の合計サイズ</returns>
		protected int GetBodySize()
		{
			int size = 0;
			string str;

			for(int no = 0 ; no < _header_list.Count ; no++)
			{
				if(no < _body_list.Count)
				{
					str = (string)_body_list[no];
					size += str.Length;
				}
				str = (string)_header_list[no];
				size += str.Length;
			}
			return size;
		}
		/// <summary>
		/// 添付ファイルを展開して保存します。
		/// </summary>
		/// <exception cref="FormatException">
		///	<see cref="Add"/>で元となる文字列（ヘッダ,本文)が追加されていません。
		///	</exception>
		/// <exception cref="DirectoryNotFoundException">
		///	<see cref="Path"/>で指定したフォルダが存在しません。
		/// </exception>
		/// <exception cref="nMailException">
		///	展開中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Save()
		{
			if(_path != null)
			{
				if(_path != "" && !Directory.Exists(_path))
				{
					throw new DirectoryNotFoundException(_path);
				}
			}
			if(_header_list.Count != _body_list.Count && _body_list.Count > 0)
			{
				throw new FormatException();
			}
			string read_body;
			string read_header;
			string keep_id = "";
			int id_no;

			SetHeaderSize();
			_subject = new StringBuilder(_header_size);
			_date = new StringBuilder(_header_size);
			_from = new StringBuilder(_header_size);
			_header = new StringBuilder(_header_size);
			_body = new StringBuilder(GetBodySize());
			Options.FileNameMax = MaxPath;
			_filename = new StringBuilder(MaxPath);
			for(int no = 0 ; no < _header_list.Count ; no++)
			{
				if(no < _body_list.Count)
				{
					read_body = (string)_body_list[no];
				}
				else
				{
					read_body = "";
				}
				read_header = (string)_header_list[no];
				if(no == 0)
				{
					id_no = GetId(read_header);
					if(id_no > 1) {
						throw new nMailException("Save", ErrorInvalidNo);
					}
					keep_id = PartId;
					_attachment_flag = true;
					_err = AttachmentFileFirst(_temp, _subject, _date, _from, _header, _body, _path, _filename, read_header, read_body);
				}
				else
				{
					if(GetId(read_header) != no + 1 || keep_id != PartId) {
						throw new nMailException("Save", ErrorInvalidNo);
					}
					_err = AttachmentFileNextVB(_temp, _subject, _date, _from, _header, _body, _path, _filename, read_header, read_body);
				}
				if(_err < 0 && _err != ErrorPartial)
				{
					throw new nMailException("Save", _err);
				}
			}
			if(_filename.Length > 0) 
			{
				_filename_list = _filename.ToString().Split(Options.SplitChar);
			} 
			else 
			{
				_filename_list = null;
			}
			if(Options.SaveHtmlFile == Options.SaveHtmlFileOn) {
				_html_file = new StringBuilder(_header_size);
				Pop3GetHeaderField(_html_file, _header.ToString(), "X-NMAIL-HTML-FILE:", _header_size);
			}
			if(Options.SaveRfc822File != Options.SaveRfc822FileOff) {
				_rfc822_file = new StringBuilder(_header_size);
				Pop3GetHeaderField(_rfc822_file, _header.ToString(), "X-NMAIL-RFC822-FILE:", _header_size);
			}
			AttachmentFileClose(_temp);
			_attachment_flag = false;
		}
		/// <summary>
		/// 展開元となる分割ファイルデータを内部リストに追加します。
		/// </summary>
		/// <param name="header_body">ヘッダー＋本文の文字列</param>
		/// <remarks>
		///	ヘッダと本文の間には一行の空行が必要です。
		/// </remarks>
		public void Add(string header_body)
		{
			_header_list.Add(header_body);
		}
		/// <summary>
		/// 展開元となる分割ファイルデータを内部リストに追加します。
		/// </summary>
		/// <param name="header">ヘッダーの文字列</param>
		/// <param name="body">本文の文字列</param>
		public void Add(string header, string body)
		{
			_header_list.Add(header);
			_body_list.Add(body);
		}
		/// <summary>
		/// 分割ファイルの ID を取得します。
		/// </summary>
		/// <param name="header">ヘッダーの文字列</param>
		/// <returns><see cref="NoAttachmentFile"/>の場合分割されていない添付ファイル付きメールです。
		/// 1 以上の場合、分割ファイルの番号となります。
		/// </returns>
		public int GetId(string header)
		{
			_id = new StringBuilder(header.Length);
			return AttachmentFileStatus(header, _id, header.Length);
		}
		/// <summary>
		/// メールヘッダから指定のフィールドの内容を取得します。
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <returns>フィールドの内容
		/// </returns>
		public string GetHeaderField(string field_name)
		{
			_field_name = field_name;
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			_err = Pop3GetHeaderField(_field, _header.ToString(), _field_name, _header_size);
			if(_err < 0)
			{
				throw new nMailException("GetHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// メールヘッダから指定のフィールドの内容を取得します。
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <param name="header">ヘッダ</param>
		/// <returns>フィールドの内容
		/// </returns>
		public string GetHeaderField(string field_name, string header)
		{
			_field_name = field_name;
			_header = new StringBuilder(header);
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			_err = Pop3GetHeaderField(_field, _header.ToString(), _field_name, _header_size);
			if(_err < 0)
			{
				throw new nMailException("GetHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// MIME ヘッダフィールドの文字列をデコードします
		/// </summary>
		/// <param name="field">フィールドの文字列</param>
		/// <returns>デコードしたフィールド内容</returns>
		public string DecodeHeaderField(string field)
		{
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			_err = Pop3DecodeHeaderField(_field, field, _header_size);
			if(_err < 0)
			{
				throw new nMailException("DecodeHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// メールヘッダから指定のヘッダフィールドの内容を取得し、
		/// MIME ヘッダフィールドのデコードを行って返します
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <returns>取得したデコード済みのフィールド内容</returns>
		public string GetDecodeHeaderField(string field_name)
		{
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			String src = GetHeaderField(field_name, _header.ToString());
			_err = Pop3DecodeHeaderField(_field, src, _header_size);
			if(_err < 0)
			{
				throw new nMailException("DecodeHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// メールヘッダから指定のヘッダフィールドの内容を取得し、
		/// MIME ヘッダフィールドのデコードを行って返します
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <param name="header">ヘッダ</param>
		/// <returns>取得したデコード済みのフィールド内容</returns>
		public string GetDecodeHeaderField(string field_name, string header)
		{
			SetHeaderSize();
			_err = Pop3DecodeHeaderField(_field, GetHeaderField(field_name, header), _header_size);
			if(_err < 0)
			{
				throw new nMailException("DecodeHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// 添付ファイル名の配列です。
		/// </summary>
		///	<remarks>
		/// このプロパティは互換性のために残してあります。
		///	<see cref="GetFileNameList"/>で配列を取得して使用するようにしてください。
		///	</remarks>
		/// <returns>添付ファイル名の配列</returns>
		public string[] GetFileNameList()
		{
			if(_filename_list == null)
			{
				return new string[0];
			}
			else
			{
				return (string[])_filename_list.Clone();
			}
		}

		/// <summary>
		/// 件名
		/// </summary>
		/// <value>メール件名</value>
		public string Subject
		{
			get
			{
				if(_subject == null)
				{
					return "";
				}
				else
				{
					return _subject.ToString();
				}
			}
		}
		/// <summary>
		/// メールの送信日時の文字列です。
		/// </summary>
		/// <value>メール送信日時文字列</value>
		public string DateString
		{
			get
			{
				if(_date == null)
				{
					return "";
				}
				else
				{
					return _date.ToString();
				}
			}
		}
		/// <summary>
		/// メールの差出人です。
		/// </summary>
		/// <value>メール差出人</value>
		public string From
		{
			get
			{
				if(_from == null)
				{
					return "";
				}
				else
				{
					return _from.ToString();
				}
			}
		}
		/// <summary>
		/// メールの本文です。
		/// </summary>
		/// <value>メール本文</value>
		public string Body
		{
			get
			{
				if(_body == null)
				{
					return "";
				}
				else
				{
					return _body.ToString();
				}
			}
		}
		/// <summary>
		/// 添付ファイルを保存するフォルダです。
		/// </summary>
		/// <remarks>
		/// null (VB.Net は nothing) の場合保存しません。
		/// </remarks>
		/// <value>添付ファイル保存フォルダ</value>
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}
		/// <summary>
		/// 分割ファイルの ID です。
		/// </summary>
		/// <value>分割ファイル ID</value>
		public string PartId
		{
			get
			{
				if(_id == null)
				{
					return "";
				}
				else
				{
					return _id.ToString();
				}
			}
		}
		/// <summary>
		/// 添付ファイル名です。
		/// </summary>
		/// <remarks>
		/// 複数の添付ファイルがある場合、"," で区切られて格納されます。
		/// <see cref="Options.SplitChar"/>で区切り文字を変更できます。
		/// </remarks>
		/// <value>添付ファイル名</value>
		public string FileName
		{
			get
			{
				if(_filename == null)
				{
					return "";
				}
				else
				{
					return _filename.ToString();
				}
			}
		}
		/// <summary>
		/// 添付ファイル名の配列です。
		/// </summary>
		///	<remarks>
		/// このプロパティは互換性のために残してあります。
		///	<see cref="GetFileNameList"/>で配列を取得して使用するようにしてください。
		///	</remarks>
		/// <value>添付ファイル名の配列</value>
		public string[] FileNameList
		{
			get
			{
				return _filename_list;
			}
		}
		/// <summary>
		/// エラー番号
		/// </summary>
		/// <value>エラー番号</value>
		public int ErrorCode
		{
			get
			{
				return _err;
			}
		}
		/// <summary>
		/// text/html パートを保存したファイルの名前です。
		/// </summary>
		/// <value>ファイル名</value>
		public string HtmlFile
		{
			get
			{
				if(_html_file == null)
				{
					return "";
				}
				else
				{
					return _html_file.ToString();
				}
			}
		}
		/// <summary>
		/// メールのヘッダです。
		/// </summary>
		/// <value>メールのヘッダ</value>
		public string Header
		{
			get
			{
				if(_header == null)
				{
					return "";
				}
				else
				{
					return _header.ToString();
				}
			}
			set
			{
				_header = new StringBuilder(value);
			}
		}
		/// <summary>
		/// ヘッダフィールドの内容です。
		/// </summary>
		/// <value>ヘッダフィールドの内容</value>
		public string Field
		{
			get
			{
				if(_field == null)
				{
					return "";
				}
				else
				{
					return _field.ToString();
				}
			}
		}
		/// <summary>
		/// message/rfc822 パートを保存したファイルの名前です。
		/// </summary>
		/// <value>ファイル名</value>
		public string Rfc822File
		{
			get
			{
				if(_rfc822_file == null)
				{
					return "";
				}
				else
				{
					return _rfc822_file.ToString();
				}
			}
		}
	}
	/// <summary>
	/// nMail.DLL 設定用クラス
	/// </summary>
	public class Options
	{
		/// <summary>
		/// <c>Options</c>クラスの新規インスタンスを初期化します。
		/// </summary>
		public Options()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		protected const int MessageSize = 32768;

		/// <summary>
		/// オプション：タイムアウト
		/// </summary>
		protected const int OptionTimeout = 0;
		/// <summary>
		/// オプション：ヘッダサイズ
		/// </summary>
		protected const int OptionHeaderMax = 1;
		/// <summary>
		/// オプション：本文サイズ
		/// </summary>
		protected const int OptionBodyMax = 2;
		/// <summary>
		/// オプション：上書きモード
		/// </summary>
		protected const int OptionAlreadyFile = 3;
		/// <summary>
		/// オプション：デバックモード
		/// </summary>
		protected const int OptionDebugMode = 4;
		/// <summary>
		/// オプション：接続時タイムアウト
		/// </summary>
		protected const int OptionConnectTimeout = 5;
		/// <summary>
		/// オプション：区切り文字
		/// </summary>
		protected const int OptionSplitChar = 6;
		/// <summary>
		/// オプション：一時休止サイズ
		/// </summary>
		protected const int OptionSuspendSize = 7;
		/// <summary>
		/// オプション：Sleep 時間
		/// </summary>
		protected const int OptionSleepTime = 8;
		/// <summary>
		/// オプション：フィールドサイズ
		/// </summary>
		protected const int OptionFieldMax = 9;
		/// <summary>
		/// オプション：text/html パート保存
		/// </summary>
		protected const int OptionSaveHtmlFile = 10;
		/// <summary>
		/// オプション：本文のデコード
		/// </summary>
		protected const int OptionDecodeBody = 11;
		/// <summary>
		/// オプション：ヘッダのデコード
		/// </summary>
		protected const int OptionDecodeHeader = 12;
		/// <summary>
		/// オプション：添付ファイル名格納バッファサイズ
		/// </summary>
		protected const int OptionFileNameMax = 13;
		/// <summary>
		/// オプション：添付ファイル名に使用できない文字を置き換える文字
		/// </summary>
		protected const int OptionChangeChar = 14;
		/// <summary>
		/// オプション：添付ファイル名を置き換える条件
		/// </summary>
		protected const int OptionChangeCharMode = 15;
		/// <summary>
		/// オプション：message/rfc822 パート保存
		/// </summary>
		protected const int OptionSaveRfc822File = 16;

		/// <summary>
		/// 同名ファイルを上書きします。
		/// </summary>
		public const int FileOverwrite = 0;
		/// <summary>
		/// ファイル名の末尾(拡張子の前)に数字を追加して保存します。
		/// "file.dat" が存在する場合、"file1.dat" という名前で保存します。
		/// </summary>
		public const int FileRename = 1;
		/// <summary>
		/// ファイルを作成せずエラーとします。
		/// </summary>
		public const int FileAlreadyError = 2;

		/// <summary>
		/// エラーメッセージを取得します。
		/// </summary>
		protected const int MessageError = 0;
		/// <summary>
		/// デバッグ用のサーバとの交信文字列を取得します。
		/// </summary>
		protected const int MessageDebug = 1;

		/// <summary>
		/// デバックモードが有効です。
		/// </summary>
		public const int DebugStart = 1;
		/// <summary>
		/// デバックモードが無効です。(初期値)
		/// </summary>
		public const int DebugEnd = 0;

		/// <summary>
		/// 接続タイムアウトが有効です。
		/// </summary>
		public const int ConnectTimeoutOn = 1;
		/// <summary>
		/// 接続タイムアウトが無効です。(初期値)
		/// </summary>
		public const int ConnectTimeoutOff = 0;

		/// <summary>
		/// text/html パートをファイルに保存します。
		/// </summary>
		public const int SaveHtmlFileOn = 1;
		/// <summary>
		/// text/html パートをファイルに保存しません。(初期値)
		/// </summary>
		public const int SaveHtmlFileOff = 0;

		/// <summary>
		/// 本文は全てデコードします。(初期値)
		/// </summary>
		public const int DecodeBodyOn = 0;
		/// <summary>
		/// 本文は全てデコードしません。本文のテキストも iso-2022-jp のままとなります。
		/// </summary>
		public const int DecodeBodyAllOff = 1;
		/// <summary>
		/// テキストパートの Content-Transfer-Encoding 指定のエンコードのみデコードしません。
		/// </summary>
		public const int DecodeBodyTextOff = 2;
		/// <summary>
		/// ヘッダは全てデコードします。(初期値)
		/// </summary>
		public const int DecodeHeaderOn = 0;
		/// <summary>
		/// ヘッダは全てデコードしません。
		/// </summary>
		public const int DecodeHeaderOff = 1;

		/// <summary>
		/// 使用不可な文字と区切り文字を置き換えます。(初期値)
		/// </summary>
		public const int ChangeSplitChar = 0;
		/// <summary>
		/// 半角スペースを置き換えます。
		/// </summary>
		public const int ChangeHalfSpace = 1;
		/// <summary>
		/// 全角スペースを置き換えます。
		/// </summary>
		public const int ChangeFullSpace = 2;
		/// <summary>
		/// 半角・全角スペースを置き換えます。
		/// </summary>
		public const int ChangeAllSpace = 3;
		/// <summary>
		/// 区切り文字を置き換えません。
		/// </summary>
		public const int ChangeNoSplitChar = 4;

		/// <summary>
		/// message/rfc822 パートをまとめてファイルに保存します。
		/// </summary>
		public const int SaveRfc822FileAllOn = 2;
		/// <summary>
		/// message/rfc822 パートのテキスト部分をファイルに保存します。
		/// </summary>
		public const int SaveRfc822FileBodyOn = 1;
		/// <summary>
		/// message/rfc822 パートをファイルに保存しません。
		/// </summary>
		public const int SaveRfc822FileOff = 0;

		[DllImport("nMail.DLL", EntryPoint="NMailGetVersion")]
		protected static extern int GetVersion();

		[DllImport("nMail.DLL", EntryPoint="NMailGetOption")]
		protected static extern int GetOption(int option);

		[DllImport("nMail.DLL", EntryPoint="NMailSetOption")]
		protected static extern int SetOption(int option, int value);

		[DllImport("nMail.DLL", EntryPoint="NMailGetMessage", CharSet=CharSet.Auto)]
		protected static extern void GetMessage(int Type, StringBuilder Message, int Size);

		[DllImport("nMail.DLL", EntryPoint="NMailGetSuspendNumber", CharSet=CharSet.Auto)]
		protected static extern int GetSuspendNumber(string Path);

		/// <summary>
		/// デバックモードを開始します。
		/// </summary>
		/// <remarks>
		/// <see cref="EndDebug"/>を呼び出した後に <see cref="DebugMessage"/>
		/// を参照することによってサーバとの交信文字列を取得できます。
		/// </remarks>
		public static void StartDebug()
		{
			SetOption(OptionDebugMode, DebugStart);
		}

		/// <summary>
		/// デバックモードを終了します。
		/// </summary>
		/// <remarks>
		/// 呼び出し後に <see cref="DebugMessage"/> を参照することによってサーバ
		/// との交信文字列を取得できます。
		/// </remarks>
		public static void EndDebug()
		{
			SetOption(OptionDebugMode, DebugEnd);
		}

		/// <summary>
		/// 接続時タイムアウト設定を有効にします。
		/// </summary>
		public static void EnableConnectTimeout()
		{
			SetOption(OptionConnectTimeout, ConnectTimeoutOn);
		}

		/// <summary>
		/// 接続時タイムアウト設定を無効にします。(初期値)
		/// </summary>
		public static void DisableConnectTimeout()
		{
			SetOption(OptionConnectTimeout, ConnectTimeoutOff);
		}
		/// <summary>
		/// text/html パートをファイルに保存します。
		/// </summary>
		public static void EnableSaveHtmlFile()
		{
			SetOption(OptionSaveHtmlFile, SaveHtmlFileOn);
		}
		/// <summary>
		/// text/html パートをファイルに保存しません。(初期値)
		/// </summary>
		public static void DisableSaveHtmlFile()
		{
			SetOption(OptionSaveHtmlFile, SaveHtmlFileOff);
		}
		/// <summary>
		/// 本文をデコードします。(初期値)
		/// </summary>
		public static void EnableDecodeBody()
		{
			SetOption(OptionDecodeBody, DecodeBodyOn);
		}
		/// <summary>
		/// 本文をデコードしません。本文のテキストも iso-2022-jp のままとなります。
		/// </summary>
		public static void DisableDecodeBodyAll()
		{
			SetOption(OptionDecodeBody, DecodeBodyAllOff);
		}
		/// <summary>
		/// テキストパートの Content-Transfer-Encoding 指定のエンコードのみデコードしません。
		/// </summary>
		public static void DisableDecodeBodyText()
		{
			SetOption(OptionDecodeBody, DecodeBodyTextOff);
		}
		/// <summary>
		/// ヘッダをデコードします。(初期値)
		/// </summary>
		public static void EnableDecodeHeader()
		{
			SetOption(OptionDecodeHeader, DecodeHeaderOn);
		}
		/// <summary>
		/// ヘッダをデコードしません。
		/// </summary>
		public static void DisableDecodeHeader()
		{
			SetOption(OptionDecodeHeader, DecodeHeaderOff);
		}
		/// <summary>
		/// message/rfc822 パートをまとめてファイルに保存します。
		/// </summary>
		public static void EnableSaveRfc822FileAll()
		{
			SetOption(OptionSaveRfc822File, SaveRfc822FileAllOn);
		}
		/// <summary>
		/// message/rfc822 パートのテキスト部分をファイルに保存します。
		/// </summary>
		public static void EnableSaveRfc822FileBody()
		{
			SetOption(OptionSaveRfc822File, SaveRfc822FileBodyOn);
		}
		/// <summary>
		/// message/rfc822 パートをファイルに保存しません。（初期値）
		/// </summary>
		public static void DisableSaveRfc822File()
		{
			SetOption(OptionSaveRfc822File, SaveRfc822FileOff);
		}
		/// <summary>
		/// 指定されたファイルを添付したメールを送信する場合の休止回数を得る
		/// </summary>
		/// <param name="file_name">添付ファイル名</param>
		/// <exception cref="FileNotFoundException">
		/// file_name で設定されているファイルが存在しません。
		/// </exception>
		/// <value>指定されたファイルを添付したメールを送信する場合の休止回数を得る</value>
		public static int SuspendCount(String file_name)
		{
			if(!File.Exists(file_name)) {
				throw new FileNotFoundException(file_name);
			}
			return GetSuspendNumber(file_name);
		}

		/// <summary>
		/// バージョンの数値です。
		/// </summary>
		/// <remarks>
		/// nMail.DLL のバージョンの数値です。
		/// Version 1.23 の場合 123 となります。
		/// </remarks>
		/// <value>バージョン数値</value>
		public static int VersionInt
		{
			get
			{
				return GetVersion();
			}
		}
		/// <summary>
		/// バージョンの文字列です。
		/// </summary>
		/// <remarks>
		/// nMail.DLL のバージョンの文字列です。
		/// Version 1.23 の場合 "1.23" となります。
		/// </remarks>
		/// <value>バージョン文字列</value>
		public static string Version
		{
			get
			{
				string str;
				int ver = GetVersion();
				str = String.Format("{0:d}.{1:d}", ver / 100, ver % 100);
				return str;
			}
		}
		/// <summary>
		/// エラーメッセージです。
		/// </summary>
		/// <remarks>
		/// エラー発生時にサーバから返ってきたエラーメッセージです。
		/// </remarks>
		/// <value>エラーメッセージ</value>
		public static string ErrorMessage
		{
			get
			{
				StringBuilder message;
				message = new StringBuilder(MessageSize);
				GetMessage(MessageError, message, MessageSize);
				return message.ToString();
			}
		}
		/// <summary>
		/// デバック用文字列です。
		/// </summary>
		/// <remarks>
		/// <see cref="StartDebug"/>を呼び出した後にメールの送受信処理を行い、
		/// <see cref="EndDebug"/>を呼び出した後にこのプロパティを参照すること
		/// によってサーバとの交信文字列を取得することができます。
		/// </remarks>
		/// <value>デバック用文字列</value>
		public static string DebugMessage
		{
			get
			{
				StringBuilder message;
				message = new StringBuilder(MessageSize);
				GetMessage(MessageDebug, message, MessageSize);
				return message.ToString();
			}
		}
		/// <summary>
		/// 応答タイムアウト値です。
		/// </summary>
		/// <remarks>
		/// この設定値より長い時間サーバから応答が無い場合、
		/// タイムアウトエラーとして処理を返します。
		/// 単位は ms です。
		/// 接続時のタイムアウトは通常は無効です。
		/// 有効にしたい場合、<see cref="EnableConnectTimeout"/>
		/// を呼び出してください。
		/// </remarks>
		/// <value>応答タイムアウト値</value>
		public static int Timeout
		{
			get
			{
				return GetOption(OptionTimeout);
			}
			set
			{
				SetOption(OptionTimeout, value);
			}
		}
		/// <summary>
		/// ヘッダバッファサイズです。
		/// </summary>
		/// <remarks>
		/// メール受信や添付ファイル展開時にヘッダデータを格納する
		/// バッファのサイズです。
		/// <see cref="Pop3"/>クラスでメールの受信を行う場合、内部で
		/// バッファサイズの設定を行いますので、このプロパティを
		/// 使用する必要はありません。
		/// </remarks>
		/// <value>ヘッダバッファサイズ</value>
		public static int HeaderMax
		{
			get
			{
				return GetOption(OptionHeaderMax);
			}
			set
			{
				SetOption(OptionHeaderMax, value);
			}
		}
		/// <summary>
		/// 本文バッファサイズです。
		/// </summary>
		/// <remarks>
		/// メール受信や添付ファイル展開時にヘッダデータを格納する
		/// 本文のサイズです。
		/// <see cref="Pop3"/>クラスでメールの受信を行う場合、内部で
		/// メールサイズに応じたバッファサイズの指定を行いますので、
		/// このプロパティを使用する必要はありません。
		/// </remarks>
		/// <value>本文バッファサイズ</value>
		public static int BodyMax
		{
			get
			{
				return GetOption(OptionBodyMax);
			}
			set
			{
				SetOption(OptionBodyMax, value);
			}
		}
		/// <summary>
		/// 添付ファイル保存時に、指定フォルダに同名ファイルがあった場合の処理方法です。
		/// </summary>
		/// <remarks>
		/// <see cref="FileOverwrite"/>同名ファイルを上書きします。
		/// <see cref="FileRename"/>ファイル名の末尾(拡張子の前)に数字を追加して保存します。
		/// <see cref="FileAlreadyError"/>ファイルを作成せずエラーとします。
		/// </remarks>
		/// <value>添付ファイル保存時の同名ファイル処理方法</value>
		public static int AlreadyFile
		{
			get
			{
				return GetOption(OptionAlreadyFile);
			}
			set
			{
				SetOption(OptionAlreadyFile, value);
			}
		}
		/// <summary>
		/// デバックモード
		/// </summary>
		/// <remarks>
		/// <see cref="DebugStart"/>デバックモードが有効です。
		/// <see cref="DebugEnd"/>デバックモードが無効です。
		/// </remarks>
		/// <value>デバックモード</value>
		public static int DebugMode
		{
			get
			{
				return GetOption(OptionDebugMode);
			}
			set
			{
				SetOption(OptionDebugMode, value);
			}
		}
		/// <summary>
		/// サーバ接続時のタイムアウトが有効かどうかの設定です。
		/// </summary>
		/// <remarks>
		/// <see cref="ConnectTimeoutOn"/>接続タイムアウトが有効です。
		/// <see cref="ConnectTimeoutOff"/>接続タイムアウトが無効です。
		/// </remarks>
		/// <value>サーバ接続時が有効かどうかの設定値</value>
		public static int ConnectTimeout
		{
			get
			{
				return GetOption(OptionConnectTimeout);
			}
			set
			{
				SetOption(OptionConnectTimeout, value);
			}
		}
		/// <summary>
		/// 添付ファイルの区切り文字です。
		/// </summary>
		/// <remarks>
		/// 添付ファイルが複数ある場合の区切り文字です。
		///	デフォルト値は , です。
		/// </remarks>
		/// <value>添付ファイル区切り文字</value>
		public static char SplitChar
		{
			get
			{
				return (char)GetOption(OptionSplitChar);
			}
			set
			{
				SetOption(OptionSplitChar, (int)value);
			}
		}
		/// <summary>
		/// 送受信の際に一旦処理を戻すデータサイズです。
		/// </summary>
		/// <remarks>
		/// 添付ファイルがある場合、ここで指定したサイズのデータを
		/// 送受信すると、一旦処理を戻します。
		/// </remarks>
		/// <value>送受信の際に一旦処理を戻すデータサイズ</value>
		public static int SuspendSize
		{
			get
			{
				return GetOption(OptionSuspendSize);
			}
			set
			{
				SetOption(OptionSuspendSize, value);
			}
		}
		/// <summary>
		/// サーバからの返答待ちの際に入れる Sleep() の時間です。
		/// </summary>
		/// <value>サーバからの返答待ちの際に入れる Sleep() の時間</value>
		public static int SleepTime
		{
			get
			{
				return GetOption(OptionSleepTime);
			}
			set
			{
				SetOption(OptionSleepTime, value);
			}
		}
		/// <summary>
		/// フィールドバッファのサイズです。
		/// </summary>
		/// <value>フィールドバッファのサイズ</value>
		public static int FieldMax
		{
			get
			{
				return GetOption(OptionFieldMax);
			}
			set
			{
				SetOption(OptionFieldMax, value);
			}
		}
		/// <summary>
		/// HTML メールの text/html パートをファイルを保存するかどうかの設定です。
		/// </summary>
		/// <remarks>
		/// <see cref="SaveHtmlFileOff"/>text/html パートをファイルに保存しない(初期値)
		/// <see cref="SaveHtmlFileOn"/>text/html パートをファイルに保存する
		/// Attachment クラスを使って text/html パートをファイルに保存する場合、
		/// <see cref="DisableDecodeBodyText()"/> または <see cref="DisableDecodeBodyAll()"/> を呼び出した後で <see cref="Pop3.GetMail"/> で読み出した
		/// 本文データを使用し、<see cref="Attachment.Save()"/> の前に <see cref="EnableDecodeBody()"/>
		/// を実行しておく必要があります。
		/// </remarks>
		/// <value>text/html パートをファイルに保存するかどうかの設定値</value>
		public static int SaveHtmlFile
		{
			get
			{
				return GetOption(OptionSaveHtmlFile);
			}
			set
			{
				SetOption(OptionSaveHtmlFile, value);
			}
		}
		/// <summary>
		/// 本文のデコードの設定です。
		/// </summary>
		/// <remarks>
		/// <see cref="DecodeBodyOn"/>全てデコードします。(初期値)
		/// <see cref="DecodeBodyAllOff"/>全てデコードしません。本文のテキストも iso-2022-jp のままとなります。
		/// <see cref="DecodeBodyTextOff"/>テキストパートの Content-Transfer-Encoding 指定のエンコードのみデコードしません。
		/// </remarks>
		/// <value>本文をデコードするかどうかの設定値</value>
		public static int DecodeBody
		{
			get
			{
				return GetOption(OptionDecodeBody);
			}
			set
			{
				SetOption(OptionDecodeBody, value);
			}
		}
		/// <summary>
		/// ヘッダのデコードの設定です。
		/// </summary>
		/// <remarks>
		/// <see cref="DecodeHeaderOn"/>全てデコードします。(初期値)
		/// <see cref="DecodeHeaderOff"/>全てデコードしません。
		/// </remarks>
		/// <value>ヘッダをデコードするかどうかの設定値</value>
		public static int DecodeHeader
		{
			get
			{
				return GetOption(OptionDecodeHeader);
			}
			set
			{
				SetOption(OptionDecodeHeader, value);
			}
		}
		/// <summary>
		/// 添付ファイル名格納バッファサイズです。
		/// </summary>
		/// <remarks>
		/// 添付ファイル名を格納するバッファのサイズです。
		/// <see cref="Pop3"/>や<see cref="Imap4"/>クラスでメールの受信を行ったり、
		/// <see cref="Attachment"/>クラスで添付ファイルの展開を行う場合、
		/// 内部でバッファサイズの設定を行いますので、
		/// このプロパティを使用する必要はありません。
		/// </remarks>
		/// <value>添付ファイル名格納バッファサイズ</value>
		public static int FileNameMax
		{
			get
			{
				return GetOption(OptionFileNameMax);
			}
			set
			{
				SetOption(OptionFileNameMax, value);
			}
		}
		/// <summary>
		/// 添付ファイル名に使用できない文字を置き換える文字
		/// </summary>
		/// <remarks>
		/// 添付ファイル名に使用できない文字を置き換える文字です。
		/// デフォルト値は _ です。
		/// </remarks>
		/// <value>置き換え文字</value>
		public static char ChangeChar
		{
			get
			{
				return (char)GetOption(OptionChangeChar);
			}
			set
			{
				SetOption(OptionChangeChar, (int)value);
			}
		}
		/// <summary>
		/// 添付ファイル名を置き換える条件
		/// </summary>
		/// <remarks>
		/// 添付ファイル名を置き換える条件です。
		/// デフォルト値は<see cref="ChangeSplitChar"/>で、
		/// Windows でファイル名に使用できない文字と、区切り文字に
		/// 使用している文字を置き換えます。
		/// </remarks>
		/// <value>置き換え条件</value>
		public static int ChangeCharMode
		{
			get
			{
				return GetOption(OptionChangeCharMode);
			}
			set
			{
				SetOption(OptionChangeCharMode, value);
			}
		}
		/// <summary>
		/// message/rfc822 パートをファイルに保存するかどうかの設定です。
		/// </summary>
		/// <remarks>
		/// <see cref="SaveRfc822FileOff"/>message/rfc822 パートをファイルに保存しない。(初期値)
		/// <see cref="SaveRfc822FileBodyOn"/>message/rfc822 パートのテキスト部分をファイルに保存する。
		/// <see cref="SaveRfc822FileAllOn"/>message/rfc822 パートを全てファイルに保存する。
		/// SaveRfc822FileAllOn で message/rfc822 パートを全てファイルに保存した場合、
		/// Attachment クラスを使って添付ファイルを取り出す事ができます。
		/// </remarks>
		/// <value>message/rfc822 パートをファイルに保存するかどうかの設定値</value>
		public static int SaveRfc822File
		{
			get
			{
				return GetOption(OptionSaveRfc822File);
			}
			set
			{
				SetOption(OptionSaveRfc822File, value);
			}
		}
	}

	/// <summary>
	/// IMAP4rev1 メール受信クラス
	/// </summary>
	/// <remarks>
	/// <example>
	/// <para>
	///	メールボックス("inbox")内の指定のメール番号(変数名:no)の MIME 構造を取得し、テキストであれば本文を表示、添付ファイルであれば z:\temp に保存します。
	/// <code lang="cs">
	///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
	///		try {
	///			imap.Connect();
	///			imap.Authenticate("imap4_id", "password");
	///			imap.SelectMailBox("inbox");
	///			// MIME 構造を取得
	///			imap.GetMimeStructure(no);
	///			nMail.Imap4.MimeStructureStatus [] list = imap4.GetMimeStructureList()
	///			foreach(nMail.Imap4.MimeStructureStatus mime in list)
	///			{
	///				// テキストであれば本文を取得して表示
	///				if(string.Compare(mime.Type, "text", true) == 0 &amp;&amp; string.Compare(mime.SubType, "plain", true) == 0)
	///				{
	///					// 本文取得
	///					imap.GetMimePart(no, mime.PartNo);
	///					MessageBox.Show(imap.Body);
	///				}
	///				else if(string.Compare(mime.Type, "multipart", true) != 0)
	///				{
	///					// 添付ファイルを保存
	///					imap.SaveMimePart(no, mime.PartNo, @"z:\temp\" + mime.FileName);
	///				}
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
	///	Try
	///		imap.Connect()
	///		imap.Authenticate("imap4_id", "password")
	///		imap.SelectMailBox("inbox")
	///		' MIME 構造を取得
	///		imap.GetMimeStructure(no)
	///		Dim list As nMail.Imap4.MimeStructureStatus() = imap4.GetMimeStructureList()
	///		For Each mime As nMail.Imap4.MimeStructureStatus In list
	///			' テキストであれば本文を取得して表示
	///			If (String.Compare(mime.Type, "text", True) = 0) And (String.Compare(mime.SubType, "plain", True) = 0) Then
	///				' 本文取得
	///				imap.GetMimePart(no, mime.PartNo)
	///				MessageBox.Show(imap.Body)
	///			Else If String.Compare(mime.Type, "multipart", true) &lt;&gt; 0 Then
	///				' 添付ファイルを保存
	///				imap.SaveMimePart(no, mime.PartNo, "z:\temp\" + mime.FileName)
	///			End If
	///		Next
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		imap.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// メールボックス("inbox")内の指定のメール番号(変数名:no)を取得し、件名と本文を表示する。
	/// 後ほど Attachment クラスで添付ファイルを展開する場合かつ text/html パートをファイルに保存したい場合、
	/// <see cref="Options.DisableDecodeBodyText()"/> もしくは <see cref="Options.DisableDecodeBodyAll()"/>を呼んでから<see cref="GetMail"/>で取得したヘッダおよび本文データを使用する必要があります。
	/// <code lang="cs">
	///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
	///		try {
	///			imap.Connect();
	///			imap.Authenticate("imap4_id", "password");
	///		    imap.SelectMailBox("inbox");
	///			imap.GetMail(no);
	///			MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}\r\n{2:s}", no, imap.Subject, imap.Body));
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
	/// Try
	/// 	imap.Connect()
	/// 	imap.Authenticate("imap4_id", "password")
	///		imap.SelectMailBox("inbox")
	///		imap.GetMail(no)
	///		MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}" + ControlChars.CrLf + "{2:s}", no, imap.Subject, imap.Body))
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		imap.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// STARTTLS を使用してメールボックス("inbox")内の指定のメール番号(変数名:no)を取得し、件名と本文を表示する。
	/// <code lang="cs">
	///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
	///		try {
	///			imap.SSL = nMail.Imap4.STARTTLS;
	///			imap.Connect();				// STARTTLS の場合通常の IMAP4 ポート番号で接続
	///			imap.Authenticate("imap4_id", "password");
	///		    imap.SelectMailBox("inbox");
	///			imap.GetMail(no);
	///			MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}\r\n{2:s}", no, imap.Subject, imap.Body));
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
	/// Try
	/// 	imap.SSL = nMail.Imap4.STARTTLS
	/// 	imap.Connect()					' STARTTLS の場合通常の IMAP4 ポート番号で接続
	/// 	imap.Authenticate("imap4_id", "password")
	///		imap.SelectMailBox("inbox")
	///		imap.GetMail(no)
	///		MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}" + ControlChars.CrLf + "{2:s}", no, imap.Subject, imap.Body))
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		imap.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// メールボックス("inbox")内の指定のメール番号(変数名:no)を取得し、件名と本文を表示する。添付ファイルを z:\temp に保存する。
	/// text/html パートをファイルに保存する場合、<see cref="GetMail"/> の前に <see cref="Options.EnableSaveHtmlFile()"/> を呼んでおく必要があります。
	/// 保存した text/html パートのファイル名は <see cref="HtmlFile"/> で取得できます。
	/// <code lang="cs">
	///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
	///		try {
	///			imap.Connect();
	///			imap.Authenticate("imap4_id", "password");
	///		    imap.SelectMailBox("inbox");
	///			imap.Path = @"z:\temp";
	///			imap.GetMail(no);
	///			MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}\r\n{2:s}", no, imap.Subject, imap.Body));
	///			string [] file_list = imape.GetFileNameList();
	///			if(file_list.Length == 0)
	///			{
	///				MessageBox.Show("ファイルはありません");
	///			}
	///			else
	///			{
	///				foreach(string name in file_list)
	///				{
	///					MessageBox.Show(String.Format("添付ファイル名:{0:s}", name));
	///				}
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
	/// Try
	/// 	imap.Connect()
	/// 	imap.Authenticate("imap4_id", "password")
	///		imap.SelectMailBox("inbox")
	///		imap.Path = "z:\temp"
	///		imap.GetMail(no)
	///		MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}" + ControlChars.CrLf + "{2:s}", no, imap.Subject, imap.Body))
	///		Dim file_list As String() = imap.GetFileNameList()
	///		If file_list.Length = 0 Then
	///			MessageBox.Show("ファイルはありません")
	///		Else
	///			For Each name As String In file_list
	///				MessageBox.Show(String.Format("添付ファイル名:{0:s}", name))
	///			Next fno
	///		End If
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		imap.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	///	一時休止機能を使ってメールボックス("inbox")内の指定のメール番号(変数名:no)を取得し、件名と本文を表示する。添付ファイルを z:\temp に保存する。
	/// <code lang="cs">
	///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
	///		try {
	///			imap.Connect();
	///			imap.Authenticate("imap4_id", "password");
	///			imap.SelectMailBox("inbox");
	///			// だいたいの休止回数を得る
	///			int count = imap.GetSize(no) / (nMail.Options.SuspendSize * 1024) + 1;
	///			imap.Path = @"z:\temp";
	///			imap.GetMail(no);
	///			imap.Flag = nMail.Imap4.SuspendAttachmentFile;
	///			imap.GetMail(no);
	///			imap.Flag = nMail.Imap4.SuspendNext;
	///			while(imap.ErrorCode == nMail.Imap4.ErrorSuspendAttachmentFile)
	///			{
	///				imap.GetMail(no);
	///				// プログレスバーを進める等の処理
	///				Application.DoEvents();
	///			}
	///			MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}\r\n{2:s}", no, imap.Subject, imap.Body));
	///			string [] file_list = imape.GetFileNameList();
	///			if(file_list.Length == 0)
	///			{
	///				MessageBox.Show("ファイルはありません");
	///			}
	///			else
	///			{
	///				foreach(string name in file_list)
	///				{
	///					MessageBox.Show(String.Format("添付ファイル名:{0:s}", name));
	///				}
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
	///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
	///	Try
	///		Dim count As Integer
	///
	///		imap.Connect()
	///		imap.Authenticate("imap4_id", "password")
	///		imap.SelectMailBox("inbox");
	///		' だいたいの休止回数を得る
	///		count = imap.GetSize(no) \ (nMail.Options.SuspendSize * 1024) + 1
	///		imap.Path = "z:\temp"
	///		imap.Flag = nMail.Imap4.SuspendAttachmentFile
	///		imap.GetMail(no)
	///		imap.Flag = nMail.Imap4.SuspendNext
	///		Do While imap.ErrorCode = nMail.Imap4.ErrorSuspendAttachmentFile
	///			imap.GetMail(no)
	///			' プログレスバーを進める等の処理
	///			Application.DoEvents()
	///		Loop
	///		MessageBox.Show(String.Format("メール番号:{0:d} 件名:{1:s}" + ControlChars.CrLf + "{2:s}", no, imap.Subject, imap.Body))
	///		Dim file_list As String() = imap.GetFileNameList()
	///		If file_list.Length = 0 Then
	///			MessageBox.Show("ファイルはありません")
	///		Else
	///			For Each name As String In file_list
	///				MessageBox.Show(String.Format("添付ファイル名:{0:s}", name))
	///			Next fno
	///		End If
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
	///	Finally
	///		imap.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// </example>
	/// </remarks>
	public class Imap4 : IDisposable
	{
		/// <summary>
		/// ヘッダ領域のサイズです。
		/// </summary>
		protected const int HeaderSize = 32768;
		/// <summary>
		/// パス文字列のサイズです。
		/// </summary>
		protected const int MaxPath = 32768;
		/// <summary>
		/// ソケットエラーもしくは未接続状態です。値は -1 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Connect"/>を呼び出さずに<see cref="GetMail"/>等を、</para>
		/// <para>呼び出したり、なんらかの理由で接続が切断されるとこのエラーとなります。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorSocket = -1;
		/// <summary>
		/// 認証エラーです。値は -2 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Authenticate"/>呼び出しで認証に失敗した場合</para>
		/// <para>このエラーとなります。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorAuthenticate = -2;
		/// <summary>
		/// 番号で指定されたメールが存在しません。値は -3 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>で指定した番号のメールが存在しないとこのエラーとなります。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorInvalidNo = -3;
		/// <summary>
		/// タイムアウトエラーです。値は -4 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Options.Timeout"/>で指定した値より長い時間サーバから応答が</para>
		/// <para>ない場合、このエラーが発生します。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorTimeout = -4;
		/// <summary>
		/// 添付ファイルが開けません。値は -5 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Path"/>で指定したフォルダに添付ファイルが書き込めない</para>
		/// <para>ない場合、このエラーが発生します。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorFileOpen = -5;
		/// <summary>
		/// 添付ファイルと同名のファイルがフォルダに存在します。値は -7 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="nMail.Options.AlreadyFile"/> に <see cref="nMail.Options.FileAlreadyError"/></para>
		/// <para>を設定し、<see cref="Path"/>で指定したフォルダに添付ファイルと同じ名前のファイルが</para>
		/// <para>ある場合にこのエラーが発生します。</para>
		/// <para><see cref="nMailException.ErrorCode"/>に設定されます。</para>
		/// </remarks>
		public const int ErrorFileAlready = -7;
		/// <summary>
		/// サーバが指定した認証形式に対応していません。値は -8 です。
		/// </summary>
		public const int ErrorAuthenticateNoSupport = -8;
		/// <summary>
		/// メモリ確保エラーです。値は -9 です。
		/// </summary>
		/// <remarks>
		/// 内部で文字コードを変換するためのメモリを確保できませんでした。
		/// </remarks>
		public const int ErrorMemory = -9;
		/// <summary>
		/// その他のエラーです。値は -10 です。
		/// </summary>
		public const int ErrorEtc = -10;
		/// <summary>
		/// パラメータが正しくありません。値は -11 です。
		/// </summary>
		/// <remarks>
		/// パラメータが正しくありません。
		/// </remarks>
		public const int ErrorInvalidParameter = -11;
		/// <summary>
		/// IMAP4 サーバ応答エラー。値は -12 です。
		/// </summary>
		/// <remarks>
		/// IMAP4 サーバが NO を返しました。
		/// </remarks>
		public const int ErrorResponseNo = -12;
		/// <summary>
		/// IMAP4 サーバ応答エラー。値は -13 です。
		/// </summary>
		/// <remarks>
		/// IMAP4 サーバが Bad を返しました。
		/// </remarks>
		public const int ErrorResponseBad = -13;
		/// <summary>
		/// 名前空間が存在しません。値は -14 です。
		/// </summary>
		public const int ErrorNoNameSpace = -14;
		/// <summary>
		/// 添付ファイル受信中でまだ残りがある状態です。値は -20 です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="Flag"/>に<see cref="SuspendAttachmentFile"/>または</para>
		/// <para><see cref="SuspendNext"/>を指定し<see cref="GetMail"/></para>
		/// <para>を呼び出した場合、まだメールの残りがある場合、<see cref="nMailException.ErrorCode"/></para>
		/// <para>に設定されます。</para>
		/// </remarks>
		public const int ErrorSuspendAttachmentFile = -20;
		/// <summary>
		/// 該当する項目がありません。値は -21 です。
		/// </summary>
		/// <remarks>
		/// 該当する項目は存在しません。
		/// </remarks>
		public const int ErrorNoResult = -21;
		/// <summary>
		/// 認証で PLAIN を使用します。
		/// </summary>
		public const int AuthPlain = 1;
		/// <summary>
		/// 認証で LOGIN を使用します。
		/// </summary>
		public const int AuthLogin = 2;
		/// <summary>
		/// 認証で CRAM MD5 を使用します。
		/// </summary>
		public const int AuthCramMd5 = 4;
		/// <summary>
		/// 認証で DIGEST MD5 を使用します。
		/// </summary>
		public const int AuthDigestMd5 = 8;
		/// <summary>
		/// 実行成功
		/// </summary>
		/// <para>各種メソッドが成功を返した場合、<see cref="nMailException.ErrorCode"/>に設定される値です。</para>
		public const int Success = 1;
		/// <summary>
		/// 添付ファイル受信で一時休止ありの一回目に指定します。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>の前に、<see cref="Flag"/>に設定しておきます。</para>
		/// </remarks>
		public const int SuspendAttachmentFile = 4;
		/// <summary>
		/// 添付ファイル受信で一時休止ありの二回目以降に指定します。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>の前に、<see cref="Flag"/>に設定しておきます。</para>
		/// </remarks>
		public const int SuspendNext = 8;
		/// <summary>
		/// メール追加時に日時フィールドを nMail.DLL 内で生成します。
		/// </summary>
		public const int AddDateField = 1;
		/// <summary>
		/// メール追加時にMessage-ID フィールドを nMail.DLL 内で生成します。
		/// </summary>
		public const int AddMessageId = 2;
		/// <summary>
		/// MIME パート保存で引数のパスでファイル名も指定します。
		/// </summary>
		private const int MimeSaveAsFile = 0x0800;
		/// <summary>
		/// MIME パート取得で既読フラグを立てません。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMimePart"/>の前に、<see cref="MimeFlag"/>に設定しておきます。</para>
		/// </remarks>
		public const int MimePeek = 0x1000;
		/// <summary>
		/// MIME パート取得でデコードしません。
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMimePart"/>の前に、<see cref="MimeFlag"/>に設定しておきます。</para>
		/// </remarks>
		public const int MimeNoDecode = 0x2000;
		/// <summary>
		/// MIME パート取得でヘッダのみ取得します。
		/// </summary>
		private const int MimeHeader = 0x4000;
		/// <summary>
		/// メール番号ではなく UID を使用します。
		/// </summary>
		public const int UseUidValue = 0x8000;
		/// <summary>
		/// メールボックスフラグ　選択できないメールボックスです。
		/// </summary>
		public const int MailBoxNoSelect = 1;
		/// <summary>
		/// メールボックスフラグ　子メールボックスを作成できません。
		/// </summary>
		public const int MailBoxNoInferious = 2;
		/// <summary>
		/// メールボックスフラグ　メッセージが追加されている可能性があります。
		/// </summary>
		public const int MailBoxMarked = 4;
		/// <summary>
		/// メールボックスフラグ　メッセージが追加されていません。
		/// </summary>
		public const int MailBoxUnMarked = 8;
		/// <summary>
		/// メールボックスフラグ　子メールボックスが存在します。
		/// </summary>
		public const int MailBoxChildren = 16;
		/// <summary>
		/// メールボックスフラグ　子メールボックスが存在しません。
		/// </summary>
		public const int MailBoxNoChildren = 32;
		/// <summary>
		/// メールメッセージフラグ　返信済みフラグ付きです。
		/// </summary>
		public const int MessageAnswerd = 1;
		/// <summary>
		/// メールメッセージフラグ　消去マーク付きです。
		/// </summary>
		public const int MessageDeleted = 2;
		/// <summary>
		/// メールメッセージフラグ　草稿フラグ付きです。
		/// </summary>
		public const int MessageDraft = 4;
		/// <summary>
		/// メールメッセージフラグ　重要性フラグ付きです。
		/// </summary>
		public const int MessageFlagged = 8;
		/// <summary>
		/// メールメッセージフラグ　到着フラグ付きです。
		/// </summary>
		public const int MessageRecent = 16;
		/// <summary>
		/// メールメッセージフラグ　既読フラグ付きです。
		/// </summary>
		public const int MessageSeen = 32;
		/// <summary>
		/// メール検索　返信済み
		/// </summary>
		public const int SearchAnswered = 1;
		/// <summary>
		/// メール検索　返信済みでない
		/// </summary>
		public const int SearchUnAnswered = 2;
		/// <summary>
		/// メール検索　消去マーク付き
		/// </summary>
		public const int SearchDeleted = 3;
		/// <summary>
		/// メール検索　消去マークなし
		/// </summary>
		public const int SearchUnDeleted = 4;
		/// <summary>
		/// メール検索　草稿フラグあり
		/// </summary>
		public const int SearchDraft = 5;
		/// <summary>
		/// メール検索　草稿フラグなし
		/// </summary>
		public const int SearchUnDraft = 6;
		/// <summary>
		/// メール検索　重要性フラグあり
		/// </summary>
		public const int SearchFlagged = 7;
		/// <summary>
		/// メール検索　重要性フラグなし
		/// </summary>
		public const int SearchUnFlagged = 8;
		/// <summary>
		/// メール検索　到着フラグあり
		/// </summary>
		public const int SearchRecent = 9;
		/// <summary>
		/// メール検索　到着フラグなし
		/// </summary>
		public const int SearchUnRecent = 10;
		/// <summary>
		/// メール検索　既読フラグあり
		/// </summary>
		public const int SearchSeen = 11;
		/// <summary>
		/// メール検索　既読フラグなし
		/// </summary>
		public const int SearchUnSeen = 12;
		/// <summary>
		/// メール検索　到着フラグありかつ既読フラグなし
		/// </summary>
		public const int SearchNew = 13;
		/// <summary>
		/// メール検索　全てのメール
		/// </summary>
		public const int SearchAll = 14;
		/// <summary>
		/// メール検索　指定されたキーワードあり
		/// </summary>
		public const int SearchKeyword = 15;
		/// <summary>
		/// メール検索　指定されたキーワードなし
		/// </summary>
		public const int SearchUnKeyword = 16;
		/// <summary>
		/// メール検索　From ヘッダに指定の文字列あり
		/// </summary>
		public const int SearchFrom = 17;
		/// <summary>
		/// メール検索　To ヘッダに指定の文字列あり
		/// </summary>
		public const int SearchTo = 18;
		/// <summary>
		/// メール検索　CC ヘッダに指定の文字列あり
		/// </summary>
		public const int SearchCc = 19;
		/// <summary>
		/// メール検索　BCC ヘッダに指定の文字列あり
		/// </summary>
		public const int SearchBcc = 20;
		/// <summary>
		/// メール検索　SUBJECT ヘッダに指定の文字列あり
		/// </summary>
		public const int SearchSubject = 21;
		/// <summary>
		/// メール検索　DATE ヘッダが指定された年月日以前
		/// </summary>
		public const int SearchSentBefore = 22;
		/// <summary>
		/// メール検索　DATE ヘッダが指定された年月日
		/// </summary>
		public const int SearchSentOn = 23;
		/// <summary>
		/// メール検索　DATE ヘッダが指定された年月日以降
		/// </summary>
		public const int SearchSentSince = 24;
		/// <summary>
		/// メール検索　指定された年月日以前に到着
		/// </summary>
		public const int SearchBefore = 25;
		/// <summary>
		/// メール検索　指定された年月日に到着
		/// </summary>
		public const int SearchOn = 26;
		/// <summary>
		/// メール検索　指定された年月日以降に到着
		/// </summary>
		public const int SearchSince = 27;
		/// <summary>
		/// メール検索　指定されたヘッダフィールドに文字列を含む 検索文字列に"ヘッダフィールド 文字列"と指定してください
		/// </summary>
		public const int SearchHeader = 28;
		/// <summary>
		/// メール検索　本文に文字列を含む
		/// </summary>
		public const int SearchBody = 29;
		/// <summary>
		/// メール検索　ヘッダおよび本文に文字列を含む
		/// </summary>
		public const int SearchText = 30;
		/// <summary>
		/// メール検索　指定サイズより大きなメール
		/// </summary>
		public const int SearchLager = 31;
		/// <summary>
		/// メール検索　指定サイズより小さなメール
		/// </summary>
		public const int SearchSmaller = 32;
		/// <summary>
		/// メール検索　指定 UID のメール
		/// </summary>
		public const int SearchUid = 33;
		/// <summary>
		/// メール検索　検索文字列を直接指定
		/// </summary>
		public const int SearchCommand = 0x0100;
		/// <summary>
		/// メール検索　検索条件 NOT
		/// </summary>
		public const int SearchNot = 0x0200;
		/// <summary>
		/// メール検索　検索条件 OR
		/// </summary>
		public const int SearchOr = 0x0400;
		/// <summary>
		/// メール検索　検索条件 AND
		/// </summary>
		public const int SearchAnd = 0x0800;
		/// <summary>
		/// メール検索　検索条件ワーク初期化
		/// </summary>
		public const int SearchFirst = 0x1000;
		/// <summary>
		/// メール検索　検索コマンド発行後すぐに戻る
		/// </summary>
		public const int SearchNoWait = 0x2000;
		/// <summary>
		/// メール検索　検索結果チェック
		/// </summary>
		public const int SearchCheck = 0x4000;
		/// <summary>
		/// 個人名前空間
		/// </summary>
		private const int NameSpacePersonal = 0;
		/// <summary>
		/// 他人名前空間
		/// </summary>
		private const int NameSpaceOther = 1;
		/// <summary>
		/// 共有名前空間
		/// </summary>
		private const int NameSpaceShared = 2;
		/// <summary>
		/// 名前空間数
		/// </summary>
		private const int NameSpaceMax = 3;
		/// <summary>
		/// 個人名前空間 数取得
		/// </summary>
		private const int NameSpacePersonalCount = 0x1000;
		/// <summary>
		/// 他人名前空間 数取得
		/// </summary>
		private const int NameSpaceOtherCount = 0x2000;
		/// <summary>
		/// 共有名前空間 数取得
		/// </summary>
		private const int NameSpaceSharedCount = 0x3000;
		/// <summary>
		/// 個人名前空間 個別取得
		/// </summary>
		private const int NameSpacePersonalNo = 0x0100;
		/// <summary>
		/// 他人名前空間 個別取得
		/// </summary>
		private const int NameSpaceOtherNo = 0x0200;
		/// <summary>
		/// 共有名前空間 個別取得
		/// </summary>
		private const int NameSpaceSharedNo = 0x0300;
		/// <summary>
		/// メールメッセージフラグを追加します。
		/// </summary>
		private const int AddMessage = 1;
		/// <summary>
		/// メールメッセージフラグを削除します。
		/// </summary>
		private const int DeleteMessage = 2;
		/// <summary>
		/// メールメッセージフラグを置き換えます。
		/// </summary>
		private const int ReplaceMessage = 3;
		/// <summary>
		/// メールボックスの総メッセージ数を取得します。
		/// </summary>
		private const int MailBoxMessage = 1;
		/// <summary>
		/// メールボックスの新着メッセージ数を取得します。
		/// </summary>
		private const int MailBoxRecent = 2;
		/// <summary>
		/// メールボックスの新しいメッセージの UID を取得します。
		/// </summary>
		private const int MailBoxNextUid = 3;
		/// <summary>
		/// メールボックスの UID Validity 値を取得します。
		/// </summary>
		private const int MailBoxUidValidity = 4;
		/// <summary>
		/// メールボックスの未読メッセージ数を取得します。
		/// </summary>
		private const int MailBoxUnSeen = 5;
		/// <summary>
		/// ボディ構造 番号文字列
		/// </summary>
		private const int ContentPart = 0;
		/// <summary>
		/// ボディ構造 Content-Type タイプ
		/// </summary>
		private const int ContentType = 1;
		/// <summary>
		/// ボディ構造 Content-Type サブタイプ
		/// </summary>
		private const int ContentSubType = 2;
		/// <summary>
		/// ボディ構造 Content-Id
		/// </summary>
		private const int ContentId = 3;
		/// <summary>
		/// ボディ構造 Content-Description
		/// </summary>
		private const int ContentDescription = 4;
		/// <summary>
		/// ボディ構造 Content-Transfer-Encoding
		/// </summary>
		private const int ContentTransferEncoding = 5;
		/// <summary>
		/// ボディ構造 ファイル名
		/// </summary>
		private const int ContentFileName = 6;
		/// <summary>
		/// ボディ構造 Content-Type タイプ＆サブタイプ
		/// </summary>
		private const int ContentTypeAndSubType = 7;
		/// <summary>
		/// ボディ構造 ボディサイズ
		/// </summary>
		private const int ContentSize = 8;
		/// <summary>
		/// ボディ構造 ボディ行数
		/// </summary>
		private const int ContentLine = 9;
		/// <summary>
		/// ボディ構造 Content-Type パラメータ数
		/// </summary>
		private const int ContentParameterCount = 10;
		/// <summary>
		/// ボディ構造 Content-Type パラメータ
		/// </summary>
		private const int ContentParameter = 11;
		/// <summary>
		/// SMTP 日時文字列
		/// </summary>
		private const int MakeDateSmtp = 0x00000;
		/// <summary>
		/// IMAP4 日時文字列
		/// </summary>
		private const int MakeDateImap4 = 0x10000;
		/// <summary>
		/// 現在日時
		/// </summary>
		private const int MakeDateNow = 0x20000;
		/// <summary>
		/// タイムゾーン指定 ＋
		/// </summary>
		private const int MakeDateTimeZonePlus = 0x40000;
		/// <summary>
		/// タイムゾーン指定 ー
		/// </summary>
		private const int MakeDateTimeZoneMinus = 0x80000;
		/// <summary>
		/// SMTP日時文字列(RFC2822)を取得します。例: "Fri, 9 Jul 2009 15:10:30 +0900
		/// </summary>
		private const int DateTimeSmtp = 0x0000;
		/// <summary>
		/// IMAP4日付文字列(RFC2060)を取得します。検索用 例: "09-Jul-2009"
		/// </summary>
		private const int DateImap4 = 0x8000;
		/// <summary>
		/// IMAP4日時文字列(RFC2060)を取得します。Append用 例: "09-Jul-2009 15:10:30"
		/// </summary>
		private const int DateTimeImap4 = 0x8000;
		/// <summary>
		/// 日時文字列のバッファサイズです。
		/// </summary>
		private const int DateStringSize = 34;
		/// <summary>
		/// SSLv3 を使用します。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int SSL3 = 0x1000;
		/// <summary>
		/// TSLv1 を使用します。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int TLS1 = 0x2000;
		/// <summary>
		/// STARTTLS を使用します。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int STARTTLS = 0x4000;
		/// <summary>
		/// サーバ証明書が期限切れでもエラーにしません。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int IgnoreNotTimeValid = 0x0800;
		/// <summary>
		/// ルート証明書が無効でもエラーにしません。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int AllowUnknownCA = 0x0400;
		/// <summary>
		/// Common Name が一致しなくてもエラーにしません。
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>で指定します。
		/// </remarks>
		public const int IgnoreInvalidName = 0x0040;
		/// <summary>
		/// IMAP4 の標準ポート番号です
		/// </summary>
		public const int StandardPortNo = 143;
		/// <summary>
		/// IMAP4 over SSL のポート番号です
		/// </summary>
		public const int StandardSslPortNo = 993;

		/// <summary>
		/// IMAP4 ポート番号です。
		/// </summary>
		protected int _port = 143;
		/// <summary>
		/// ソケットハンドルです。
		/// </summary>
		protected IntPtr _socket = (IntPtr)ErrorSocket;
		/// <summary>
		/// メール数です。
		/// </summary>
		protected int _count = -1;
		/// <summary>
		/// メールサイズです。
		/// </summary>
		protected int _size = -1;
		/// <summary>
		/// ヘッダーサイズです。
		/// </summary>
		protected int _header_size = -1;
		/// <summary>
		/// 本文のサイズです。
		/// </summary>
		protected int _body_size = -1;
		/// <summary>
		/// メール受信時の設定用フラグです。
		/// </summary>
		protected int _flag = 0;
		/// <summary>
		/// メールメッセージのフラグです。
		/// </summary>
		protected int _message_flag = 0;
		/// <summary>
		/// メールを指定する際に UID を使用します。
		/// </summary>
		protected bool _useuid_flag = false;
		/// <summary>
		/// MIME パート取得もしくは保存の際の設定フラグです。
		/// </summary>
		protected int _mime_flag = 0;
		/// <summary>
		/// メールを指定する際に UID を使用します。
		/// </summary>
		protected int _useuid = 0;
		/// <summary>
		/// エラー番号です。
		/// </summary>
		protected int _err = 0;
		/// <summary>
		/// IMAP4 認証形式
		/// </summary>
		protected int _mode = AuthLogin;
		/// <summary>
		/// IMAP4 サーバ名です。
		/// </summary>
		protected string _host = "";
		/// <summary>
		/// IMAP4 ユーザー名です。
		/// </summary>
		protected string _id = "";
		/// <summary>
		/// IMAP4 パスワードです。
		/// </summary>
		protected string _password = "";
		/// <summary>
		/// メールボックス名です。
		/// </summary>
		protected string _mailbox = "";
		/// <summary>
		/// 添付ファイル保存用のパスです。
		/// </summary>
		protected string _path = null;
		/// <summary>
		/// ヘッダフィールド名です。
		/// </summary>
		protected string _field_name = "";
		/// <summary>
		/// 本文格納バッファです。
		/// </summary>
		protected StringBuilder _body = null;
		/// <summary>
		/// 件名格納バッファです。
		/// </summary>
		protected StringBuilder _subject = null;
		/// <summary>
		/// 日付文字列保存バッファです。
		/// </summary>
		protected StringBuilder _date = null;
		/// <summary>
		/// 差出人格納バッファです。
		/// </summary>
		protected StringBuilder _from = null;
		/// <summary>
		/// ヘッダ格納バッファです。
		/// </summary>
		protected StringBuilder _header = null;
		/// <summary>
		/// 添付ファイル名格納バッファです。
		/// </summary>
		protected StringBuilder _filename = null;
		/// <summary>
		/// 添付ファイル名のリストです。
		/// </summary>
		protected string[] _filename_list = null;
		/// <summary>
		/// ヘッダフィールド内容格納バッファです。
		/// </summary>
		protected StringBuilder _field = null;
		/// <summary>
		/// UID です。
		/// </summary>
		protected uint _uid = 0;
		/// <summary>
		/// Dispose 処理を行ったかどうかのフラグです。
		/// </summary>
		private bool _disposed = false;
		/// <summary>
		/// text/html パートを保存したファイルの名前です。
		/// </summary>
		protected StringBuilder _html_file = null;
		/// <summary>
		/// SSL 設定フラグです。
		/// </summary>
		protected int _ssl = 0;
		/// <summary>
		/// SSL クライアント証明書名です。
		/// </summary>
		protected string _cert_name = null;
		/// <summary>
		/// message/rfc822 パートを保存したファイルの名前です。
		/// </summary>
		protected StringBuilder _rfc822_file = null;

		/// <summary>
		/// メールボックス情報です。
		/// </summary>
		/// <remarks>
		/// <see cref="GetMailBoxList"/>もしくは<see cref="GetMailBoxListSubscribe"/>の結果が格納される構造体です。
		/// </remarks>
		public struct MailBoxStatus
		{
			/// <summary>
			/// メールボックスフラグです。
			/// </summary>
			/// <remarks>
			/// <para><see cref="MailBoxNoSelect"/>選択できないメールボックスです</para>
			/// <para><see cref="MailBoxNoInferious"/>子メールボックスを作成できません</para>
			/// <para><see cref="MailBoxMarked"/>メッセージが追加されている可能性があります</para>
			/// <para><see cref="MailBoxUnMarked"/>メッセージが追加されていません</para>
			/// <para><see cref="MailBoxChildren"/>子メールボックスが存在します</para>
			/// <para><see cref="MailBoxNoChildren"/>子メールボックスが存在しません</para>
			/// </remarks>
			public int Flag;
			/// <summary>
			/// メールボックス名です。
			/// </summary>
			public string Name;
			/// <summary>
			/// メールボックス区切り文字です。
			/// </summary>
			public char Separate;
		}
		/// <summary>
		/// メールボックス情報リストです。
		/// </summary>
		/// <remarks>
		/// <see cref="GetMailBoxList"/>もしくは<see cref="GetMailBoxListSubscribe"/>実行後に結果が格納されます。
		/// </remarks>
		protected MailBoxStatus[] _mailbox_list = null;
		/// <summary>
		/// MIME 構造 Content-Type フィールドのパラメータ情報です。
		/// </summary>
		/// <remarks>
		/// MIME 構造 Content-Type フィールドのパラメータ情報です。
		/// MIME 構造情報 <see cref="MimeStructureStatus"/> に含まれます。
		/// </remarks>
		public struct MimeParameterStatus
		{
			/// <summary>
			/// パラメータ名称です。
			/// </summary>
			public string Name;
			/// <summary>
			/// パラメータの値です。
			/// </summary>
			public string Value ;
		}
		/// <summary>
		/// MIME 構造情報です。
		/// </summary>
		/// <remarks>
		/// <see cref="GetMimeStructure"/>の結果が格納される構造体です。
		/// </remarks>
		public struct MimeStructureStatus
		{
			/// <summary>
			/// MIME パート番号です。
			/// </summary>
			/// <remarks>
			/// <see cref="GetMimePart"/>や<see cref="SaveMimePart"/>の part_no で指定する値です。
			/// </remarks>
			public int PartNo;
			/// <summary>
			/// MIME 構造を表す文字列です。
			/// </summary>
			public string Part;
			/// <summary>
			/// Content-Type フィールドのタイプです。
			/// </summary>
			public string Type;
			/// <summary>
			/// Content-Type フィールドのサブタイプです。
			/// </summary>
			public string SubType;
			/// <summary>
			/// Contetn-ID フィールドの値です。
			/// </summary>
			public string Id;
			/// <summary>
			/// Content-Description フィールドの値です。
			/// </summary>
			public string Description;
			/// <summary>
			/// Content-Transfer-Encoding フィールドの値です。
			/// </summary>
			public string Encoding;
			/// <summary>
			/// 添付ファイル名です。(デコード済み)
			/// </summary>
			public string FileName;
			/// <summary>
			/// MIME パートのサイズです。
			/// </summary>
			public int Size;
			/// <summary>
			/// MIME パートの行数です。タイプが text の場合のみ正しい値が入ります。
			/// </summary>
			public int Line;
			/// <summary>
			/// Content-Type のパラメータリストです。
			/// </summary>
			public MimeParameterStatus[] Parameter;
		}
		/// <summary>
		/// MIME 構造情報リストです。
		/// </summary>
		/// <remarks>
		/// <see cref="GetMimeStructure"/>実行後に結果が格納されます。
		/// </remarks>
		protected MimeStructureStatus[] _mime_list = null;
		/// <summary>
		/// 検索結果のリストです。
		/// </summary>
		/// <remarks>
		/// <see cref="Search"/>実行後に結果が格納されます。
		/// </remarks>
		protected uint[] _search_list = null;
		/// <summary>
		/// 最初の検索かどうかのフラグです。
		/// </summary>
		protected bool _search_first_flag = true;
		/// <summary>
		/// 名前空間情報です。
		/// </summary>
		/// <remarks>
		/// <see cref="GetNameSpace"/>の結果が格納される構造体です。
		/// </remarks>
		public struct NameSpaceStatus
		{
			/// <summary>
			/// 名前空間のプレフィックスです。
			/// </summary>
			public string Name;
			/// <summary>
			/// 名前空間の区切り文字です。
			/// </summary>
			public char Separate;
		}
		/// <summary>
		/// 名前空間情報リストです。
		/// </summary>
		/// <remarks>
		/// <see cref="GetNameSpace"/>実行後に結果が格納されます。
		/// </remarks>
		protected NameSpaceStatus[][] _namespace_list = new NameSpaceStatus[NameSpaceMax][];

		/// <summary>
		/// <c>Imap4</c>クラスの新規インスタンスを初期化します。
		/// </summary>
		public Imap4()
		{
			Init();
		}
		/// <summary>
		/// <c>Imap4</c>クラスの新規インスタンスを初期化します。
		/// </summary>
		/// <param name="host_name">IMAP4 サーバー名</param>
		public Imap4(string host_name)
		{
			Init();
			_host = host_name;
		}
		/// <summary>
		/// 
		/// </summary>
		~Imap4()
		{
			Dispose(false);
		}
		/// <summary>
		/// <see cref="Imap4"/>によって使用されているすべてのリソースを解放します。
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// <see cref="Imap4"/>によって使用されているすべてのリソースを解放します。
		/// </summary>
		/// <param name="disposing">
		/// マネージリソースとアンマネージリソースの両方を解放する場合は<c>true</c>。
		/// アンマネージリソースだけを解放する場合は<c>false</c>。
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			if(!_disposed) {
				if(disposing)
				{
				}
				if(_socket != (IntPtr)ErrorSocket) {
					Imap4Close(_socket);
					_socket = (IntPtr)ErrorSocket;
				}
				_disposed = true;
			}
		}
		/// <summary>
		/// 初期化処理です。
		/// </summary>
		protected void Init()
		{
		}

		[DllImport("nMail.DLL", EntryPoint="NMailImap4ConnectPortNo", CharSet=CharSet.Auto)]
		protected static extern IntPtr Imap4ConnectPortNo(string Host, int Port);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4ConnectSsl", CharSet=CharSet.Auto)]
		protected static extern IntPtr Imap4ConnectSsl(string Host, int Port, int Flag, string Name);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4Close")]
		protected static extern int Imap4Close(IntPtr Socket);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4Authenticate", CharSet=CharSet.Auto)]
		protected static extern int Imap4Authenticate(IntPtr Socket, string Id, string Pass, int Mode);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4SelectMailBox", CharSet=CharSet.Auto)]
		protected static extern int Imap4SelectMailBox(IntPtr Socket, string MailBox, bool ReadOnlyFlag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetMailStatus", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetMailStatus(IntPtr Socket, uint MailNo, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, ref int MessageFlag, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetMailSize")]
		protected static extern int Imap4GetMailSize(IntPtr Socket, uint MailNo, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetMail", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetMail(IntPtr Socket, uint MailNo, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, StringBuilder Body, string Path, StringBuilder FileName, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetMailBoxList", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetMailBoxList(IntPtr Socket, string Refer, string MailBox, bool SubscribeFlag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetMailBoxListStatus", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetMailBoxListStatus(IntPtr Socket, int No, ref int MailBoxFlag, StringBuilder Separate, StringBuilder MailBox, int Size);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4DeleteMail")]
		protected static extern int Imap4DeleteMail(IntPtr Socket, uint MailNo, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4ChangeMessageFlag")]
		protected static extern int Imap4ChangeMessageFlag(IntPtr Socket, uint MailNo, int MessageFlag, int Command, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4ExpungeMail")]
		protected static extern int Imap4ExpungeMail(IntPtr Socket);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4CloseMailBox")]
		protected static extern int Imap4CloseMailBox(IntPtr Socket);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4CreateMailBox", CharSet=CharSet.Auto)]
		protected static extern int Imap4CreateMailBox(IntPtr Socket, string MailBox);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4DeleteMailBox", CharSet=CharSet.Auto)]
		protected static extern int Imap4DeleteMailBox(IntPtr Socket, string MailBox);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4RenameMailBox", CharSet=CharSet.Auto)]
		protected static extern int Imap4RenameMailBox(IntPtr Socket, string OldMailBox, string NewMailBox);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4SubscribeMailBox", CharSet=CharSet.Auto)]
		protected static extern int Imap4SubscribeMailBox(IntPtr Socket, string MailBox);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4UnsubscribeMailBox", CharSet=CharSet.Auto)]
		protected static extern int Imap4UnsubscribeMailBox(IntPtr Socket, string MailBox);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetMailBoxStatus", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetMailBoxStatus(IntPtr Socket, string MailBox, int type, ref uint Data);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4CopyMail", CharSet=CharSet.Auto)]
		protected static extern int Imap4CopyMail(IntPtr Socket, uint MailNo, string MailBox, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetUid", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetUid(IntPtr Socket, uint MailNo, ref uint Uid);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4SearchMail", CharSet=CharSet.Auto)]
		protected static extern int Imap4SearchMail(IntPtr Socket, int Type, string Text, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetSearchMailResult", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetSearchMailResult(IntPtr Socket, int No, ref uint MailNo);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4AppendMail", CharSet=CharSet.Auto)]
		protected static extern int Imap4AppendMail(IntPtr Socket, string MailBox, string Header, string Body, string Path, int MessageFlag, string DateString, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailMakeDateString", CharSet=CharSet.Auto)]
		protected static extern int MakeDateString(StringBuilder DateString, int Year, int Month, int Day, int Hour, int Minute, int Second, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetNameSpace")]
		protected static extern int Imap4GetNameSpace(IntPtr Socket);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetNameSpaceStatus", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetNameSpaceStatus(IntPtr Socket, int Type, StringBuilder Separate, StringBuilder Name, int Size);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetMimeStructure")]
		protected static extern int Imap4GetMimeStructure(IntPtr Socket, uint MailNo, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetMimeStructureStatus", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetMimeStructureStatus(IntPtr Socket, int Type, int PartNo, int SubNo, StringBuilder Name, StringBuilder Value, int Size);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4GetMimePart", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetMimePart(IntPtr Socket, uint MailNo, int PartNo, StringBuilder Body, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4SaveMimePart", CharSet=CharSet.Auto)]
		protected static extern int Imap4SaveMimePart(IntPtr Socket, uint MailNo, int PartNo, string Path, StringBuilder FileName, int Flag);

		[DllImport("nMail.DLL", EntryPoint="NMailImap4NoOperation")]
		protected static extern int Imap4NoOperation(IntPtr Socket);

		[DllImport("nMail.DLL", EntryPoint="NMailGetHeaderField", CharSet=CharSet.Auto)]
		protected static extern int Imap4GetHeaderField(StringBuilder Field, string Header, string Name, int Size);

		[DllImport("nMail.DLL", EntryPoint="NMailDecodeHeaderField", CharSet=CharSet.Auto)]
		protected static extern int Imap4DecodeHeaderField(StringBuilder Destination, string Source, int Size);

		/// <summary>
		/// ヘッダ格納用バッファのサイズを決定します。
		/// </summary>
		protected void SetHeaderSize()
		{
			if(_header_size < 0)
			{
				_header_size = Options.HeaderMax;
			}
			if(_header_size <= 0)
			{
				_header_size = HeaderSize;
				Options.HeaderMax = _header_size;
			}
		}
		/// <summary>
		/// サーバに接続中か判定する
		/// </summary>
		/// <returns>true でサーバに接続中</returns>
		public bool IsConnected()
		{
			if(_socket == (IntPtr)ErrorSocket)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		/// <summary>
		/// IMAP4 サーバに接続します。
		/// </summary>
		/// <remarks>
		/// IMAP4 サーバに接続します。
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		///	<exception cref="nMailException">
		///	IMAP4 サーバーとの接続に失敗しました。
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect()
		{
			if(_port < 0 || _port > 65536) {
				throw new ArgumentOutOfRangeException();
			}
			//_socket = Imap4ConnectPortNo(_host, _port);
			_socket = Imap4ConnectSsl(_host, _port, _ssl, _cert_name);
			if(_socket == (IntPtr)ErrorSocket)
			{
				_err = ErrorSocket;
				_socket = (IntPtr)ErrorSocket;
				throw new nMailException("Connect", _err);
			}
		}
		/// <summary>
		/// IMAP4 サーバに接続します。
		/// </summary>
		/// <param name="host_name">IMAP4 サーバー名</param>
		/// <remarks>
		/// サーバ名を指定して IMAP4 サーバに接続します。
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		///	<exception cref="nMailException">
		///	IMAP4 サーバとの接続に失敗しました。
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect(string host_name)
		{
			_host = host_name;
			Connect();
		}
		/// <summary>
		/// IMAP4 サーバに接続します。
		/// </summary>
		/// <param name="host_name">IMAP4 サーバ名</param>
		/// <param name="port_no">ポート番号</param>
		/// <remarks>
		/// サーバ名とポート番号を指定して IMAP4 サーバに接続します。
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		///	<exception cref="nMailException">
		///	IMAP4 サーバとの接続に失敗しました。
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect(string host_name, int port_no)
		{
			_host = host_name;
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// IMAP4 サーバに接続します。
		/// </summary>
		/// <param name="port_no">ポート番号</param>
		/// <remarks>
		/// ポート番号を指定して IMAP4 サーバに接続します。
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	ポート番号が正しくありません。
		/// </exception>
		///	<exception cref="nMailException">
		///	IMAP4 サーバとの接続に失敗しました。
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Connect(int port_no)
		{
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// IMAP4 サーバとの接続を終了します。
		/// </summary>
		public void Close()
		{
			if(_socket != (IntPtr)ErrorSocket) {
				Imap4Close(_socket);
			}
			_socket = (IntPtr)ErrorSocket;
		}
		/// <summary>
		/// IMAP4 サーバ認証を行います。
		/// </summary>
		/// <remarks>
		/// ID とパスワードを指定して IMAP4 サーバ認証を行います。
		/// </remarks>
		/// <param name="id_str">IMAP4 ユーザー ID</param>
		/// <param name="pass_str">IMAP4 パスワード</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="FormatException">
		///	ID もしくはパスワードに文字列が入っていません。
		/// </exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Authenticate(string id_str, string pass_str)
		{
			if(id_str == "" || pass_str == "") {
				throw new FormatException();
			}
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_id = id_str;
			_password = pass_str;
			_body_size = -1;
			_count = Imap4Authenticate(_socket, _id, _password, _mode);
			if(_count < 0)
			{
				_err = _count;
				throw new nMailException("Authenticate: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// IMAP4 サーバ認証を行います。
		/// </summary>
		/// <remarks>
		/// ID、パスワードおよび認証形式を指定して IMAP4 サーバ認証を行います。
		/// </remarks>
		/// <param name="id_str">IMAP4 ユーザー ID</param>
		/// <param name="pass_str">IMAP4 パスワード</param>
		/// <param name="mode">認証形式
		/// <para>認証形式の設定可能な値は下記の通りです。</para>
		/// <para><see cref="AuthLogin"/> で LOGIN を使用します。</para>
		/// <para><see cref="AuthPlain"/> で PLAIN を使用します。</para>
		/// <para><see cref="AuthCramMd5"/> CRAM MD5 を使用します。</para>
		/// <para><see cref="AuthDigestMd5"/> DIGEST MD5 を使用します。</para>
		/// <para>C# は | 、VB.NET では Or で複数指定可能です。</para>
		/// <para>DIGEST MD5→CRAM MD5→PLAIN→LOGIN の優先順位で認証を試みます。</para>
		/// </param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="FormatException">
		///	ID もしくはパスワードに文字列が入っていません。
		/// </exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Authenticate(string id_str, string pass_str, int mode)
		{
			_mode = mode;
			Authenticate(id_str, pass_str);
		}

		/// <summary>
		/// 購読済みメールボックスの一覧を取得します。
		/// </summary>
		/// <param name="refer">階層位置です。</param>
		/// <param name="mailbox">ワイルドカードです。"*" で全て、"%" で同じ階層のメールボックスを検索します。</param>
		/// <remarks>
		/// 購読済みメールボックスの一覧を取得します。一覧は<see cref="GetMailBoxStatusList"/> で取得できます。。
		/// <example>
		/// <para>
		///	全ての購読済みメールボックスの一覧を表示します。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			// 購読済みメールボックス一覧を取得
		///			imap.GetMailBoxListSubscribe("", "*");
		///			nMail.Imap4.MailBoxStatus [] list = imap4.GetMailBoxStatusList()
		///			foreach(nMail.Imap4.MailBoxStatus mailbox in list)
		///			{
		///				MessageBox.Show(string.Format("メールボックス名:{0:s} 区切り文字:{1:c} フラグ:{2:d}", mailbox.Name, mailbox.Separate, mailbox.Flag));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(string.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(string.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		' 購読済みメールボックス一覧を取得
		///		imap.GetMailBoxListSubscribe("", "*")
		///		Dim list As nMail.Imap4.MailBoxStatus() = imap4.GetMailBoxList()
		///		For Each mailbox As nMail.Imap4.MailBoxStatus In list
		///			MessageBox.Show(String.Format("メールボックス名:{0:s} 区切り文字:{1:c} フラグ:{2:d}", mailbox.Name, mailbox.Separate, mailbox.Flag))
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetMailBoxListSubscribe(string refer, string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) 
			{
				throw new InvalidOperationException();
			}
			_err = Imap4GetMailBoxList(_socket, refer, mailbox, true);
			if(_err < 0)
			{
				_mailbox_list = new MailBoxStatus[0];
				throw new nMailException("GetMailBoxListSubscribe: " + Options.ErrorMessage, _err);
			}
			else
			{
				SetHeaderSize();
				_mailbox_list = new MailBoxStatus[_err];
				for(int no = 0 ; no < _err ; no++) 
				{
					StringBuilder separate = new StringBuilder(_header_size);
					StringBuilder name = new StringBuilder(_header_size);
					Imap4GetMailBoxListStatus(_socket, no, ref _mailbox_list[no].Flag, separate, name, _header_size);
					_mailbox_list[no].Name = name.ToString();
					_mailbox_list[no].Separate = separate[0];
				}
			}
		}
		/// <summary>
		/// メールボックスの一覧を取得します。
		/// </summary>
		/// <remarks>
		/// メールボックスの一覧を取得します。一覧は<see cref="GetMailBoxList"/> で取得できます。
		/// <example>
		/// <para>
		///	全てのメールボックスの一覧を表示します。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			// メールボックス一覧を取得
		///			imap.GetMailBoxList("", "*");
		///			nMail.Imap4.MailBoxStatus [] list = imap4.GetMailBoxStatusList()
		///			foreach(nMail.Imap4.MailBoxStatus mailbox in list)
		///			{
		///				MessageBox.Show(string.Format("メールボックス名:{0:s} 区切り文字:{1:c} フラグ:{2:d}", mailbox.Name, mailbox.Separate, mailbox.Flag));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(string.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(string.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		' メールボックス一覧を取得
		///		imap.GetMailBoxList("", "*")
		///		Dim list As nMail.Imap4.MailBoxStatus() = imap4.GetMailBoxList()
		///		For Each mailbox As nMail.Imap4.MailBoxStatus In list
		///			MessageBox.Show(String.Format("メールボックス名:{0:s} 区切り文字:{1:c} フラグ:{2:d}", mailbox.Name, mailbox.Separate, mailbox.Flag))
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <param name="refer">階層位置です。</param>
		/// <param name="mailbox">ワイルドカードです。"*" で全て、"%" で同じ階層のメールボックスを検索します。</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetMailBoxList(string refer, string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) 
			{
				throw new InvalidOperationException();
			}
			_err = Imap4GetMailBoxList(_socket, refer, mailbox, false);
			if(_err < 0)
			{
				_mailbox_list = new MailBoxStatus[0];
				throw new nMailException("GetMailBoxList: " + Options.ErrorMessage, _err);
			}
			else
			{
				SetHeaderSize();
				_mailbox_list = new MailBoxStatus[_err];
				for(int no = 0 ; no < _err ; no++) 
				{
					StringBuilder separate = new StringBuilder(_header_size);
					StringBuilder name = new StringBuilder(_header_size);
					Imap4GetMailBoxListStatus(_socket, no, ref _mailbox_list[no].Flag, separate, name, _header_size);
					_mailbox_list[no].Name = name.ToString();
					_mailbox_list[no].Separate = separate[0];
				}
			}
		}
		/// <summary>
		/// メールボックスのメッセージ総数を取得します。
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>パラメータでメッセージ総数を取得したいメールボックス名を指定します。
		/// </remarks>
		/// <param name="mailbox">メッセージ総数を取得したいメールボックス名です。</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		/// <returns>メッセージ総数</returns>
		public int GetMailBoxMessageCount(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) 
			{
				throw new InvalidOperationException();
			}
			uint data = 0;
			_err = Imap4GetMailBoxStatus(_socket, mailbox, MailBoxMessage, ref data);
			if(_err < 0)
			{
				throw new nMailException("GetMailBoxMessageCount: " + Options.ErrorMessage, _err);
			}
			return (int)data;
		}
		/// <summary>
		/// メールボックスの新着メッセージ数を取得します。
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>パラメータで新着メッセージ数を取得したいメールボックス名を指定します。
		/// </remarks>
		/// <param name="mailbox">新着メッセージ数を取得したいメールボックス名です。</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		/// <returns>新着メッセージ数</returns>
		public int GetMailBoxRecentCount(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) 
			{
				throw new InvalidOperationException();
			}
			uint data = 0;
			_err = Imap4GetMailBoxStatus(_socket, mailbox, MailBoxRecent, ref data);
			if(_err < 0)
			{
				throw new nMailException("GetMailBoxRecentCount: " + Options.ErrorMessage, _err);
			}
			return (int)data;
		}
		/// <summary>
		/// メールボックスの新しいメッセージの UID を取得します。
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>パラメータで新しいメッセージの UID 値を取得したいメールボックス名を指定します。
		/// </remarks>
		/// <param name="mailbox">新しいメッセージの UID 値を取得したいメールボックス名です。</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		/// <returns>新しいメッセージの UID</returns>
		public uint GetMailBoxNextUid(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) 
			{
				throw new InvalidOperationException();
			}
			uint data = 0;
			_err = Imap4GetMailBoxStatus(_socket, mailbox, MailBoxNextUid, ref data);
			if(_err < 0)
			{
				throw new nMailException("GetMailBoxNextUid: " + Options.ErrorMessage, _err);
			}
			return data;
		}
		/// <summary>
		/// メールボックスの UID Validity 値を取得します。
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>パラメータで UID Validity 値を取得したいメールボックス名を指定します。
		/// </remarks>
		/// <param name="mailbox">UID Validity 値を取得したいメールボックス名です。</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		/// <returns>メールボックスの UID Validity</returns>
		public uint GetMailBoxUidValidity(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) 
			{
				throw new InvalidOperationException();
			}
			uint data = 0;
			_err = Imap4GetMailBoxStatus(_socket, mailbox, MailBoxUidValidity, ref data);
			if(_err < 0)
			{
				throw new nMailException("GetMailBoxUidValidity: " + Options.ErrorMessage, _err);
			}
			return data;
		}
		/// <summary>
		/// メールボックスの未読メッセージ数を取得します。
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>パラメータで未読メッセージ数を取得したいメールボックス名を指定します。
		/// </remarks>
		/// <param name="mailbox">未読メッセージ数を取得したいメールボックス名です。</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		/// <returns>未読メッセージ数</returns>
		public int GetMailBoxUnSeen(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) 
			{
				throw new InvalidOperationException();
			}
			uint data = 0;
			_err = Imap4GetMailBoxStatus(_socket, mailbox, MailBoxUnSeen, ref data);
			if(_err < 0)
			{
				throw new nMailException("GetMailBoxUnSeen: " + Options.ErrorMessage, _err);
			}
			return (int)data;
		}

		/// <summary>
		/// メールボックスを選択します。
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>パラメータで選択したいメールボックス名を指定します。
		/// </remarks>
		/// <param name="mailbox">選択するメールボックス名です。</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void SelectMailBox(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_mailbox = mailbox;
			_err = Imap4SelectMailBox(_socket, _mailbox, false);
			if(_err < 0)
			{
				throw new nMailException("SelectMailBox: " + Options.ErrorMessage, _err);
			}
			else
			{
				_count = _err;
			}
		}
		/// <summary>
		/// メールボックスを読み出し専用で選択します。
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>パラメータで読み出し専用で選択したいメールボックス名を指定します。
		/// </remarks>
		/// <param name="mailbox">選択するメールボックス名です。</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void SelectMailBoxReadOnly(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_mailbox = mailbox;
			_err = Imap4SelectMailBox(_socket, _mailbox, true);
			if(_err < 0)
			{
				throw new nMailException("SelectMailBoxReadOnly: " + Options.ErrorMessage, _err);
			}
			else
			{
				_count = _err;
			}
		}

		/// <summary>
		/// メールのステータスを取得します。
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <remarks>
		/// <paramref name="no"/>パラメータでステータスを取得したいメール番号を指定します。
		/// <para>件名は<see cref="Subject"/>で取得できます。</para>
		/// <para>日付文字列は<see cref="DateString"/>で取得できます。</para>
		/// <para>差出人は<see cref="From"/>で取得できます。</para>
		/// <para>ヘッダは<see cref="Header"/>で取得できます。</para>
		/// <para>メッセージフラグは<see cref="MessageFlag"/>で取得できます。</para>
		/// <para>ステータス取得失敗の場合のエラー番号は<see cref="nMailException.ErrorCode"/>で取得できます。</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetStatus(uint no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			SetHeaderSize();
			_subject = new StringBuilder(_header_size);
			_date = new StringBuilder(_header_size);
			_from = new StringBuilder(_header_size);
			_header = new StringBuilder(_header_size);
			_err = Imap4GetMailStatus(_socket, no, _subject, _date, _from, _header, ref _message_flag, _useuid);
			if(_err < 0)
			{
				throw new nMailException("GetStatus: " + Options.ErrorMessage, _err);
			}
			else
			{
				_size = _err;
				_body_size = _size * 2;
			}
		}
		/// <summary>
		/// メールのサイズを取得します。
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <remarks>
		/// <paramref name="no"/>パラメータでメールサイズを取得したいメール番号を指定します。
		/// <example>
		/// <para>
		/// メールボックス("inbox")内のメール番号(変数名:no)のメールサイズを取得する。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.GetSize(no);
		///			MessageBox.Show(String.Format("メール番号:{0:d},サイズ:{1:d}", no, imap.Size));
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.GetMail(no)
		///		MessageBox.Show(String.Format("メール番号:{0:d},サイズ:{1:d}", no, imap.Size))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		/// <returns>メールサイズ</returns>
		public int GetSize(uint no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_size = Imap4GetMailSize(_socket, no, _useuid);
			if(_size < 0) {
				_err = _size;
				throw new nMailException("GetSize: " + Options.ErrorMessage, _err);
			} else {
				_body_size = _size * 2;
			}
			return _size;
		}

		/// <summary>
		/// メールを取得します。
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <remarks>
		/// <paramref name="no"/>パラメータでメールを取得したいメール番号を指定します。
		/// <para>添付ファイルを保存したい場合、<see cref="Path"/>に保存したいフォルダを指定しておきます。</para>
		/// <para>拡張機能を使用したい場合、<see cref="Flag"/>で設定しておきます。</para>
		/// <para>件名は<see cref="Subject"/>で取得できます。</para>
		/// <para>日付文字列は<see cref="DateString"/>で取得できます。</para>
		/// <para>差出人は<see cref="From"/>で取得できます。</para>
		/// <para>ヘッダは<see cref="Header"/>で取得できます。</para>
		/// <para>メールサイズは<see cref="Size"/>で取得できます。</para>
		/// <para>添付ファイル名は<see cref="FileName"/>で取得できます。</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="DirectoryNotFoundException">
		///	<see cref="Path"/>で指定したフォルダが存在しません。
		/// </exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetMail(uint no)
		{
			if(_socket == (IntPtr)ErrorSocket)
			{
				throw new InvalidOperationException();
			}
			if(_path != null)
			{
				if(_path != "" && !Directory.Exists(_path))
				{
					throw new DirectoryNotFoundException(_path);
				}
			}
			if((_flag & SuspendNext) != 0) {
				_err = Imap4GetMail(_socket, no, _subject, _date, _from, _header, _body, _path, _filename, _flag | _useuid);
			} else {
				SetHeaderSize();
				_subject = new StringBuilder(_header_size);
				_date = new StringBuilder(_header_size);
				_from = new StringBuilder(_header_size);
				_header = new StringBuilder(_header_size);
				Options.FileNameMax = MaxPath;
				_filename = new StringBuilder(MaxPath);
				if(_body_size < 0)
				{
					GetSize(no);
				}
				if(_body_size > 0)
				{
					_body = new StringBuilder(_body_size);
					if(_flag != 0) {
						_err = Imap4GetMail(_socket, no, _subject, _date, _from, _header, _body, _path, _filename, _flag | _useuid);
					}
					else
					{
						_err = Imap4GetMail(_socket, no, _subject, _date, _from, _header, _body, _path, _filename, _flag | _useuid);
					}
				}
			}
			if(_err != ErrorSuspendAttachmentFile)
			{
				_body_size = -1;
				if(_filename.Length > 0) 
				{
					_filename_list = _filename.ToString().Split(Options.SplitChar);
				} 
				else 
				{
					_filename_list = null;
				}
				if(Options.SaveHtmlFile == Options.SaveHtmlFileOn) {
					GetHeaderField("X-NMAIL-HTML-FILE:");
					_html_file = _field;
				}
				if(Options.SaveRfc822File != Options.SaveRfc822FileOff) {
					GetHeaderField("X-NMAIL-RFC822-FILE:");
					_rfc822_file = _field;
				}
			}
			if(_err < 0 && _err != ErrorSuspendAttachmentFile) {
				throw new nMailException("GetMail: " + Options.ErrorMessage, _err);
			} 
		}
		/// <summary>
		/// メールの削除マークを付加します。
		///	実際の削除は<see cref="Expunge"/>メソッドを呼び出した際に行われます。
		/// </summary>
		/// <param name="no">削除マークを付加するメール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <remarks>
		/// <para>メール削除マーク付加失敗の場合のエラー番号は<see cref="nMailException.ErrorCode"/>で取得できます。</para>
		/// <example>
		/// <para>
		/// メールボックス("inbox")内の指定のメール番号(変数名:no)に削除マークを付加し、実際に削除します。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.Delete(no);	// 削除マーク付加
		///			imap.Expunge();		// 削除実行
		///			MessageBox.Show(String.Format("メール番号:{0:d}を削除成功", no));
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.Delete(no)			' 削除マーク付加
		///		imap.Expunge()			' 削除実行
		///		MessageBox.Show(String.Format("メール番号:{0:d}を削除成功", no))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Delete(uint no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4DeleteMail(_socket, no, _useuid);
			if(_err < 0)
			{
				throw new nMailException("Delete: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// 削除マークのついているメールを削除します。
		/// </summary>
		/// <remarks>
		/// <para>削除マークのついているメールを削除します。</para>
		/// <para>メール削除失敗の場合のエラー番号は<see cref="nMailException.ErrorCode"/>で取得できます。</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Expunge()
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4ExpungeMail(_socket);
			if(_err < 0)
			{
				throw new nMailException("Expunge: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// メールボックスの選択を終了します。
		/// </summary>
		/// <remarks>
		/// <para>メールボックスの選択を終了します。</para>
		/// <para>メールボックスの選択終了失敗の場合のエラー番号は<see cref="nMailException.ErrorCode"/>で取得できます。</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void CloseMailBox()
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4CloseMailBox(_socket);
			if(_err < 0)
			{
				throw new nMailException("CloseMailBox: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// メールメッセージフラグを付加します。
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <param name="message_flag">メッセージフラグ
		/// <para><see cref="MessageAnswerd"/>返信済みフラグ</para>
		/// <para><see cref="MessageDeleted"/>消去マーク</para>
		/// <para><see cref="MessageDraft"/>草稿フラグ</para>
		/// <para><see cref="MessageFlagged"/>重要性フラグ</para>
		/// <para><see cref="MessageRecent"/>到着フラグ</para>
		/// <para><see cref="MessageSeen"/>既読フラグ</para>
		/// <para>C# は | 、VB.NET では Or で複数指定可能です。</para>
		///	</param>
		/// <remarks>
		/// 指定したメッセージフラグを付加します。指定していないフラグの状態はそのままです。
		/// <para>フラグ付加失敗の場合のエラー番号は<see cref="nMailException.ErrorCode"/>で取得できます。</para>
		/// <example>
		/// <para>
		/// メールボックス("inbox")内の指定のメール番号(変数名:no)に返信済みフラグを付加します。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.AddMessageFlag(no, nMail.Imap4.MessageAnswerd);	// 返信済みフラグ付加
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.AddMessageFlag(no, nMail.Imap4.MessageAnswerd)		' 返信済みフラグ付加
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void AddMessageFlag(uint no, int message_flag)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4ChangeMessageFlag(_socket, no, message_flag, AddMessage, _useuid);
			if(_err < 0)
			{
				throw new nMailException("AddMessageFlag: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// メールメッセージフラグを削除します。
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <param name="message_flag">メッセージフラグ
		/// <para><see cref="MessageAnswerd"/>返信済みフラグ</para>
		/// <para><see cref="MessageDeleted"/>消去マーク</para>
		/// <para><see cref="MessageDraft"/>草稿フラグ</para>
		/// <para><see cref="MessageFlagged"/>重要性フラグ</para>
		/// <para><see cref="MessageRecent"/>到着フラグ</para>
		/// <para><see cref="MessageSeen"/>既読フラグ</para>
		/// <para>C# は | 、VB.NET では Or で複数指定可能です。</para>
		///	</param>
		/// <remarks>
		/// 指定したメッセージフラグを削除します。指定していないフラグの状態はそのままです。
		/// <para>メールメッセージフラグ付加失敗の場合のエラー番号は<see cref="nMailException.ErrorCode"/>で取得できます。</para>
		/// <example>
		/// <para>
		/// メールボックス("inbox")内の指定のメール番号(変数名:no)から削除マークフラグを削除します。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.DeleteMessageFlag(no, nMail.Imap4.MessageDeleted);	// 削除マークフラグ削除
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.DeleteMessageFlag(no, nMail.Imap4.MessageDeleted)		' 削除マークフラグ削除
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void DeleteMessageFlag(uint no, int message_flag)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4ChangeMessageFlag(_socket, no, message_flag, DeleteMessage, _useuid);
			if(_err < 0)
			{
				throw new nMailException("DeleteMessageFlag: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// メールメッセージフラグを置き換えます。
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <param name="message_flag">メッセージフラグ
		/// <para><see cref="MessageAnswerd"/>返信済みフラグ</para>
		/// <para><see cref="MessageDeleted"/>消去マーク</para>
		/// <para><see cref="MessageDraft"/>草稿フラグ</para>
		/// <para><see cref="MessageFlagged"/>重要性フラグ</para>
		/// <para><see cref="MessageRecent"/>到着フラグ</para>
		/// <para><see cref="MessageSeen"/>既読フラグ</para>
		/// <para>C# は | 、VB.NET では Or で複数指定可能です。</para>
		///	</param>
		/// <remarks>
		/// 指定したメッセージフラグで置き換えます。
		/// <para>メールメッセージフラグ置き換え失敗の場合のエラー番号は<see cref="nMailException.ErrorCode"/>で取得できます。</para>
		/// <example>
		/// <para>
		/// メールボックス("inbox")内の指定のメール番号(変数名:no)を既読および重要メッセージフラグが付いている状態とします。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.ReplaceMessageFlag(no, nMail.Imap4.MessageFlagged | nMail.Imap4.MessageSeen);	// 既読および重要性フラグ
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.ReplaceMessageFlag(no, nMail.Imap4.MessageFlagged Or nMail.Imap4.MessageSeen)	' 既読及び重要性フラグ
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void ReplaceMessageFlag(uint no, int message_flag)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4ChangeMessageFlag(_socket, no, message_flag, ReplaceMessage, _useuid);
			if(_err < 0)
			{
				throw new nMailException("ReplaceMessageFlag: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// 添付ファイルの存在チェックを行います。
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <remarks>
		///	分割されているメールについては一度受信後、Attachment クラスで結合してください。
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <returns>true で添付ファイルが存在</returns>
		public bool GetAttachmentFileStatus(uint no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			SetHeaderSize();
			_err = Imap4GetMimeStructure(_socket, no, _useuid);
			if(_err < 0)
			{
				throw new nMailException("GetAttachmentFileStatus: " + Options.ErrorMessage, _err);
			}
			else
			{
				if(_err > 1)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// メールの UID を取得します。
		/// </summary>
		/// <param name="no">メール番号</param>
		/// <remarks>
		/// <para>取得した UID は返値もしくは <see cref="Uid"/>で取得できます。</para>
		/// <example>
		/// <para>
		/// メールボックス("inbox")内の指定のメール番号(変数名:no)の UID を取得する。
		///	<code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.GetUid(no);
		///			MessageBox.Show("Uid=" + imap.Uid.ToString());
		/// 	}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.GetUid(no)
		///		MessageBox.Show("Uid=" + imap.Uid.ToString())
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		/// <returns>メールの UID</returns>
		public uint GetUid(uint no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			SetHeaderSize();
			_err = Imap4GetUid(_socket, no, ref _uid);
			if(_err < 0)
			{
				throw new nMailException("GetUid: " + Options.ErrorMessage, _err);
			}
			return _uid;
		}
		/// <summary>
		/// メールヘッダから指定のフィールドの内容を取得します。
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <returns>フィールドの内容</returns>
		/// <remarks>
		/// IMAP4 サーバとの接続とは無関係に使用できます。
		/// <para>ヘッダは、<see cref="Header"/>で設定しておきます。
		/// <see cref="GetMail"/>で受信した直後に呼び出した場合、
		/// 受信したメールのヘッダを使用します。</para>
		/// <para>取得したフィールド内容は<see cref="Field"/>で取得できます。</para>
		/// <example>
		/// <para>
		/// メールボックス("inbox")内の指定のメール番号(変数:no)の X-Mailer ヘッダフィールドを取得する。
		///	<code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.GetMail(no);
		///			MessageBox.Show("X-Mailer:" + imap.GetHeaderField("X-Mailer:"));
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception e)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.GetMail(no)
		///		MessageBox.Show("X-Mailer:" + imap.GetHeaderField("X-Mailer:"))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		/// <returns>フィールドの内容</returns>
		public string GetHeaderField(string field_name)
		{
			_field_name = field_name;
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			_err = Imap4GetHeaderField(_field, _header.ToString(), _field_name, _header_size);
			if(_err < 0)
			{
				throw new nMailException("GetHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// メールヘッダから指定のフィールドの内容を取得します。
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <param name="header">ヘッダ</param>
		/// <returns>フィールドの内容</returns>
		public string GetHeaderField(string field_name, string header)
		{
			_field_name = field_name;
			_header = new StringBuilder(header);
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			_err = Imap4GetHeaderField(_field, _header.ToString(), _field_name, _header_size);
			if(_err < 0)
			{
				throw new nMailException("GetHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// MIME ヘッダフィールドの文字列をデコードします
		/// </summary>
		/// <param name="field">フィールドの文字列</param>
		/// <returns>デコードしたフィールド内容</returns>
		public string DecodeHeaderField(string field)
		{
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			_err = Imap4DecodeHeaderField(_field, field, _header_size);
			if(_err < 0)
			{
				throw new nMailException("DecodeHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// メールヘッダから指定のヘッダフィールドの内容を取得し、
		/// MIME ヘッダフィールドのデコードを行って返します
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <returns>取得したデコード済みのフィールド内容</returns>
		public string GetDecodeHeaderField(string field_name)
		{
			SetHeaderSize();
			_field = new StringBuilder(_header_size);
			String src = GetHeaderField(field_name, _header.ToString());
			_err = Imap4DecodeHeaderField(_field, src, _header_size);
			if(_err < 0)
			{
				throw new nMailException("DecodeHeaderField", _err);
			}
			return _field.ToString();
		}
		/// <summary>
		/// メールヘッダから指定のヘッダフィールドの内容を取得し、
		/// MIME ヘッダフィールドのデコードを行って返します
		/// </summary>
		/// <param name="field_name">フィールド名</param>
		/// <param name="header">ヘッダ</param>
		/// <returns>取得したデコード済みのフィールド内容</returns>
		public string GetDecodeHeaderField(string field_name, string header)
		{
			SetHeaderSize();
			_err = Imap4DecodeHeaderField(_field, GetHeaderField(field_name, header), _header_size);
			if(_err < 0)
			{
				throw new nMailException("DecodeHeaderField", _err);
			}
			return _field.ToString();
		}

		/// <summary>
		/// メールボックスを作成します。
		/// </summary>
		/// <param name="mailbox">作成するメールボックス名</param>
		/// <remarks>
		/// メールボックスを作成します。
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void CreateMailBox(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4CreateMailBox(_socket, mailbox);
			if(_err < 0)
			{
				throw new nMailException("CreateMailBox: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// メールボックスを削除します。
		/// </summary>
		/// <remarks>
		/// メールボックスを削除します。
		/// </remarks>
		/// <param name="mailbox">削除するメールボックス名</param>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void DeleteMailBox(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4DeleteMailBox(_socket, mailbox);
			if(_err < 0)
			{
				throw new nMailException("DeleteMailBox: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// メールボックス名を変更します。
		/// </summary>
		/// <param name="old_name">変更する前のメールボックス名</param>
		/// <param name="new_name">変更後のメールボックス名</param>
		/// <remarks>
		/// メールボックス名を変更します。
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void RenameMailBox(string old_name, string new_name)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4RenameMailBox(_socket, old_name, new_name);
			if(_err < 0)
			{
				throw new nMailException("RenameMailBox: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// メールボックスを購読します。
		/// </summary>
		/// <param name="mailbox">購読するメールボックス名</param>
		/// <remarks>
		/// メールボックスを購読します。
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void SubscribeMailBox(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4SubscribeMailBox(_socket, mailbox);
			if(_err < 0)
			{
				throw new nMailException("SubscribeMailBox: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// メールボックスの購読を解除します。
		/// </summary>
		/// <param name="mailbox">購読解除するメールボックス名</param>
		/// <remarks>
		/// メールボックスの購読を解除します。
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void UnsubscribeMailBox(string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4UnsubscribeMailBox(_socket, mailbox);
			if(_err < 0)
			{
				throw new nMailException("UnsubscribeMailBox: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// メールをコピーします。
		/// </summary>
		/// <param name="no">コピーするメール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <param name="mailbox">コピー先のメールボックス名</param>
		/// <remarks>
		/// 現在選択されているメールボックスのメールを指定のメールボックスにコピーします。
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Copy(uint no, string mailbox)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4CopyMail(_socket, no, mailbox, _useuid);
			if(_err < 0)
			{
				throw new nMailException("Copy: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// MIME 構造を取得します。
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <remarks>
		/// <para>MIME 構造は<see cref="GetMimeStructureStatusList"/>で取得できます。</para>
		/// <example>
		/// <para>
		///	メールボックス("inbox")内の指定のメール番号(変数名:no)の MIME 構造を取得し、テキストであれば本文を表示、添付ファイルであれば z:\temp に保存します。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			// MIME 構造を取得
		///			imap.GetMimeStructure(no);
		///			nMail.Imap4.MimeStructureStatus [] list = imap4.GetMimeStructureStatusList()
		///			foreach(nMail.Imap4.MimeStructureStatus mime in list)
		///			{
		///				// テキストであれば本文を取得して表示
		///				if(string.Compare(mime.Type, "text", true) == 0 &amp;&amp; string.Compare(mime.SubType, "plain", true) == 0)
		///				{
		///					// 本文取得
		///					imap.GetMimePart(no, mime.PartNo);
		///					MessageBox.Show(imap.Body);
		///				}
		///				else if(string.Compare(mime.Type, "multipart", true) != 0)
		///				{
		///					// 添付ファイルを保存
		///					imap.SaveMimePart(no, mime.PartNo, @"z:\temp\" + mime.FileName);
		///				}
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		' MIME 構造を取得
		///		imap.GetMimeStructure(no)
		///		Dim list As nMail.Imap4.MimeStructureStatus() = imap4.GetMimeStructureList()
		///		For Each mime As nMail.Imap4.MimeStructureStatus In list
		///			' テキストであれば本文を取得して表示
		///			If (String.Compare(mime.Type, "text", True) = 0) And (String.Compare(mime.SubType, "plain", True) = 0) Then
		///				' 本文取得
		///				imap.GetMimePart(no, mime.PartNo)
		///				MessageBox.Show(imap.Body)
		///			Else If String.Compare(mime.Type, "multipart", true) &lt;&gt; 0 Then
		///				' 添付ファイルを保存
		///				imap.SaveMimePart(no, mime.PartNo, "z:\temp\" + mime.FileName)
		///			End If
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetMimeStructure(uint no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4GetMimeStructure(_socket, no, _flag | _useuid);
			if(_err < 0)
			{
				_mime_list = new MimeStructureStatus[0];
				throw new nMailException("GetMimeStructure: " + Options.ErrorMessage, _err);
			}
			else
			{
				SetHeaderSize();
				_mime_list = new MimeStructureStatus[_err];
				for(int part_no = 0 ; part_no < _err ; part_no++) {
					_mime_list[part_no].PartNo = part_no;
					StringBuilder name = new StringBuilder(_header_size);
					StringBuilder value = new StringBuilder(_header_size);
					Imap4GetMimeStructureStatus(_socket, ContentTypeAndSubType, part_no, 0, name, value, _header_size);
					_mime_list[part_no].Type = name.ToString();
					_mime_list[part_no].SubType = value.ToString();
					Imap4GetMimeStructureStatus(_socket, ContentPart, part_no, 0, name, null, _header_size);
					_mime_list[part_no].Part = name.ToString();
					Imap4GetMimeStructureStatus(_socket, ContentId, part_no, 0, name, null, _header_size);
					_mime_list[part_no].Id = name.ToString();
					Imap4GetMimeStructureStatus(_socket, ContentDescription, part_no, 0, name, null, _header_size);
					_mime_list[part_no].Description = name.ToString();
					Imap4GetMimeStructureStatus(_socket, ContentTransferEncoding, part_no, 0, name, null, _header_size);
					_mime_list[part_no].Encoding = name.ToString();
					Imap4GetMimeStructureStatus(_socket, ContentFileName, part_no, 0, name, null, _header_size);
					_mime_list[part_no].FileName = name.ToString();
					_mime_list[part_no].Size = Imap4GetMimeStructureStatus(_socket, ContentSize, part_no, 0, name, null, _header_size);
					if(_mime_list[part_no].Size < 0)
					{
						_mime_list[part_no].Size = 0;
					}
					_mime_list[part_no].Line = Imap4GetMimeStructureStatus(_socket, ContentLine, part_no, 0, name, null, _header_size);
					if(_mime_list[part_no].Line < 0)
					{
						_mime_list[part_no].Line = 0;
					}
					int param_count = Imap4GetMimeStructureStatus(_socket, ContentParameterCount, part_no, 0, name, null, _header_size);
					if(param_count < 0) {
						param_count = 0;
					}
					_mime_list[part_no].Parameter = new MimeParameterStatus[param_count];
					for(int param_no = 0 ; param_no < param_count ; param_no++) {
						Imap4GetMimeStructureStatus(_socket, ContentParameter, part_no, param_no, name, value, _header_size);
						_mime_list[part_no].Parameter[param_no].Name = name.ToString();
						_mime_list[part_no].Parameter[param_no].Value = value.ToString();
					}
				}
			}
		}

		/// <summary>
		/// MIME パートを取得
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <param name="part_no">MIME パート番号</param>
		/// <remarks>
		/// <para>既読フラグを立てない場合、デコードしない場合は、<see cref="MimeFlag"/>で設定しておきます。</para>
		/// <para>取得したパート本文は<see cref="Body"/>で取得できます。</para>
		/// <para>サンプルは <see cref="GetMimeStructure"/>を参照してください。</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetMimePart(uint no, int part_no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			int size = _mime_list[part_no].Size;
			if(size < 0)
			{
				GetSize(no);
				size = _size;
			}
			_body = new StringBuilder(size * 2);
			_err = Imap4GetMimePart(_socket, no, part_no, _body, _mime_flag | _useuid);
			if(_err < 0)
			{
				throw new nMailException("GetMimePart: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// MIME パートヘッダを取得
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <param name="part_no">MIME パート番号</param>
		/// <remarks>
		/// <para>既読フラグを立てない場合、デコードしない場合は、<see cref="MimeFlag"/>で設定しておきます。</para>
		/// <para>取得したパートヘッダは<see cref="Header"/>で取得できます。</para>
		/// <para>サンプルは <see cref="GetMimeStructure"/>を参照してください。</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetMimePartHeader(uint no, int part_no)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			int size = _mime_list[part_no].Size;
			if(size < 0)
			{
				GetSize(no);
				size = _size;
			}
			_body = new StringBuilder(size * 2);
			_err = Imap4GetMimePart(_socket, no, part_no, _header, MimeHeader | _mime_flag | _useuid);
			if(_err < 0)
			{
				throw new nMailException("GetMimePartHeader: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// MIME パートを保存
		/// </summary>
		/// <param name="no">メール番号(<see cref="UseUid"/>が true の場合 UID)</param>
		/// <param name="part_no">MIME パート番号</param>
		/// <param name="path">保存するファイル名</param>
		/// <remarks>
		/// <para>既読フラグを立てない場合、デコードしない場合は、<see cref="MimeFlag"/>で設定しておきます。</para>
		/// <para>保存したファイル名は<see cref="FileName"/>で取得できます。</para>
		/// <para>サンプルは <see cref="GetMimeStructure"/>を参照してください。</para>
		/// <para>Content-Type のタイプが "multipart" の MIME パートを指定した場合、エラーになりますのでご注意下さい。</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void SaveMimePart(uint no, int part_no, string path)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			Options.FileNameMax = MaxPath;
			_filename = new StringBuilder(MaxPath);
			_err = Imap4SaveMimePart(_socket, no, part_no, path, _filename, MimeSaveAsFile | _mime_flag | _useuid);
			if(_err < 0)
			{
				throw new nMailException("SaveMimePart: " + Options.ErrorMessage, _err);
			}
		}

		/// <summary>
		/// メールの検索実行(文字列有)
		/// </summary>
		/// <param name="type">検索条件
		/// <para>検索条件の設定可能な値は下記の通りです。</para>
		/// <para><see cref="SearchKeyword"/>指定されたキーワードあり</para>
		/// <para><see cref="SearchUnKeyword"/>指定されたキーワードなし</para>
		/// <para><see cref="SearchFrom"/>From ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchTo"/>To ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchCc"/>CC ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchBcc"/>BCC ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchSubject"/>SUBJECT ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchSentBefore"/>DATE ヘッダが指定された日付以前</para>
		/// <para><see cref="SearchSentOn"/>DATE ヘッダが指定された日付</para>
		/// <para><see cref="SearchSentSince"/>DATE ヘッダが指定された日付以降</para>
		/// <para><see cref="SearchBefore"/>指定された日付以前に到着</para>
		/// <para><see cref="SearchOn"/>指定された日付に到着</para>
		/// <para><see cref="SearchSince"/>指定された日付以降に到着</para>
		/// <para><see cref="SearchHeader"/>指定されたヘッダフィールドに文字列を含む 検索文字列に"ヘッダフィールド 文字列"と指定してください</para>
		/// <para><see cref="SearchBody"/>本文に文字列を含む</para>
		/// <para><see cref="SearchText"/>ヘッダおよび本文に文字列を含む</para>
		/// <para><see cref="SearchLager"/>指定サイズより大きなメール サイズは文字列に変換してください</para>
		/// <para><see cref="SearchSmaller"/>指定サイズより小さなメール サイズは文字列に変換してください</para>
		/// <para><see cref="SearchUid"/>指定 UID のメール</para>
		/// <para><see cref="SearchCommand"/>検索文字列を直接指定</para>
		/// <para><see cref="SearchNot"/>検索条件 NOT (上記の検索条件と同時に(C# では | 、VB.NET では Or)指定する必要があります)</para>
		/// </param>
		/// <param name="text">検索文字列</param>
		/// <remarks>
		///	検索結果は <see cref="GetSearchMailResultList"/> で取得できます。
		/// <example>
		/// <para>
		///	メールボックス("inbox")内の差出人(From フィールド)に support@nanshiki.co.jp が含まれるメールを検索する
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.Search(SearchFrom, "support@nanshiki.co.jp");
		///			uint [] list = imap4.GetSearchMailResultList()
		///			foreach(uint no in list)
		///			{
		///				MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.Search(SearchFrom, "support@nanshiki.co.jp")
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Search(int type, string text)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			if(_search_first_flag)
			{
				type |= SearchFirst;
				_search_first_flag = false;
			}
			_err = Imap4SearchMail(_socket, type, text, _useuid);
			_search_first_flag = true;
			if(_err < 0)
			{
				_search_list = new uint[0];
				throw new nMailException("Search: " + Options.ErrorMessage, _err);
			}
			else
			{
				_search_list = new uint[_err];
				for(int no = 0 ; no < _err ; no++) {
					Imap4GetSearchMailResult(_socket, no, ref _search_list[no]);
				}
			}
		}
		/// <summary>
		/// メールの検索実行(文字列無)
		/// </summary>
		/// <param name="type">検索条件
		/// <para>検索条件の設定可能な値は下記の通りです。</para>
		/// <para><see cref="SearchAnswered"/>返信済み</para>
		/// <para><see cref="SearchUnAnswered"/>返信済みでない</para>
		/// <para><see cref="SearchDeleted"/>消去マークあり</para>
		/// <para><see cref="SearchUnDeleted"/>消去マークなし</para>
		/// <para><see cref="SearchDraft"/>草稿フラグあり</para>
		/// <para><see cref="SearchUnDraft"/>草稿フラグなし</para>
		/// <para><see cref="SearchFlagged"/>重要性フラグあり</para>
		/// <para><see cref="SearchUnFlagged"/>重要性フラグなし</para>
		/// <para><see cref="SearchRecent"/>到着フラグあり</para>
		/// <para><see cref="SearchUnRecent"/>到着フラグなし</para>
		/// <para><see cref="SearchSeen"/>既読フラグあり</para>
		/// <para><see cref="SearchUnSeen"/>既読フラグなし</para>
		/// <para><see cref="SearchNew"/>到着フラグありかつ既読フラグなし</para>
		/// <para><see cref="SearchAll"/>全てのメール</para>
		/// <para><see cref="SearchNot"/>検索条件 NOT (上記の検索条件と同時に(C# では | 、VB.NET では Or)指定する必要があります)</para>
		/// </param>
		/// <remarks>
		///	検索結果は <see cref="GetSearchMailResultList"/> で取得できます。
		/// <example>
		/// <para>
		///	メールボックス("inbox")内の新規のメール(到着フラグ有りかつ既読フラグなし)を検索する
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.Search(SearchNew);
		///			uint [] list = imap4.GetSearchMailResultList()
		///			foreach(uint no in list)
		///			{
		///				MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.Search(SearchNew)
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void Search(int type)
		{
			Search(type, null);
		}

		/// <summary>
		/// メールの検索 AND (文字列有)
		/// </summary>
		/// <param name="type">検索条件
		/// <para>検索条件の設定可能な値は下記の通りです。</para>
		/// <para><see cref="SearchKeyword"/>指定されたキーワードあり</para>
		/// <para><see cref="SearchUnKeyword"/>指定されたキーワードなし</para>
		/// <para><see cref="SearchFrom"/>From ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchTo"/>To ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchCc"/>CC ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchBcc"/>BCC ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchSubject"/>SUBJECT ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchSentBefore"/>DATE ヘッダが指定された日付以前</para>
		/// <para><see cref="SearchSentOn"/>DATE ヘッダが指定された日付</para>
		/// <para><see cref="SearchSentSince"/>DATE ヘッダが指定された日付以降</para>
		/// <para><see cref="SearchBefore"/>指定された日付以前に到着</para>
		/// <para><see cref="SearchOn"/>指定された日付に到着</para>
		/// <para><see cref="SearchSince"/>指定された日付以降に到着</para>
		/// <para><see cref="SearchHeader"/>指定されたヘッダフィールドに文字列を含む 検索文字列に"ヘッダフィールド 文字列"と指定してください</para>
		/// <para><see cref="SearchBody"/>本文に文字列を含む</para>
		/// <para><see cref="SearchText"/>ヘッダおよび本文に文字列を含む</para>
		/// <para><see cref="SearchLager"/>指定サイズより大きなメール サイズは文字列に変換してください</para>
		/// <para><see cref="SearchSmaller"/>指定サイズより小さなメール サイズは文字列に変換してください</para>
		/// <para><see cref="SearchUid"/>指定 UID のメール</para>
		/// <para><see cref="SearchCommand"/>検索文字列を直接指定</para>
		/// <para><see cref="SearchNot"/>検索条件 NOT (上記の検索条件と同時に(C# では | 、VB.NET では Or)指定する必要があります)</para>
		/// </param>
		/// <param name="text">検索文字列</param>
		/// <remarks>
		///	AND で検索条件を指定します。<see cref="Search"/>で検索を実行します。
		/// <example>
		/// <para>
		///	メールボックス("inbox")内の件名に"テスト"が含まれ、差出人(From フィールド)に support@nanshiki.co.jp が含まれるメールを検索する
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.AddSearchAnd(SearchSubject, "テスト");
		///			imap.Search(SearchFrom, "support@nanshiki.co.jp");
		///			uint [] list = imap4.GetSearchMailResultList()
		///			foreach(uint no in list)
		///			{
		///				MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.AddSearchAnd(SearchSubject, "テスト")
		///		imap.Search(SearchFrom, "support@nanshiki.co.jp")
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void AddSearchAnd(int type, string text)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			if(_search_first_flag)
			{
				type |= SearchFirst;
				_search_first_flag = false;
			}
			_err = Imap4SearchMail(_socket, type | SearchAnd, text, _useuid);
			if(_err < 0)
			{
				throw new nMailException("Search: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// メールの検索 AND (文字列無)
		/// </summary>
		/// <param name="type">検索条件
		/// <para>検索条件の設定可能な値は下記の通りです。</para>
		/// <para><see cref="SearchAnswered"/>返信済み</para>
		/// <para><see cref="SearchUnAnswered"/>返信済みでない</para>
		/// <para><see cref="SearchDeleted"/>消去マークあり</para>
		/// <para><see cref="SearchUnDeleted"/>消去マークなし</para>
		/// <para><see cref="SearchDraft"/>草稿フラグあり</para>
		/// <para><see cref="SearchUnDraft"/>草稿フラグなし</para>
		/// <para><see cref="SearchFlagged"/>重要性フラグあり</para>
		/// <para><see cref="SearchUnFlagged"/>重要性フラグなし</para>
		/// <para><see cref="SearchRecent"/>到着フラグあり</para>
		/// <para><see cref="SearchUnRecent"/>到着フラグなし</para>
		/// <para><see cref="SearchSeen"/>既読フラグあり</para>
		/// <para><see cref="SearchUnSeen"/>既読フラグなし</para>
		/// <para><see cref="SearchNew"/>到着フラグありかつ既読フラグなし</para>
		/// <para><see cref="SearchAll"/>全てのメール</para>
		/// <para><see cref="SearchNot"/>検索条件 NOT (上記の検索条件と同時に(C# では | 、VB.NET では Or)指定する必要があります)</para>
		/// </param>
		/// <remarks>
		///	AND で検索条件を指定します。<see cref="Search"/>で検索を実行します。
		/// <example>
		/// <para>
		///	メールボックス("inbox")内の既読のメールで 2009/8/1 以降に到着したメールを検索する
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.AddSearchAnd(SearchSeen);
		///			imap.Search(SearchSince, MakeDateString(new DateTime(2009, 8, 1));
		///			uint [] list = imap4.GetSearchMailResultList()
		///			foreach(uint no in list)
		///			{
		///				MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.AddSearchAnd(SearchSeen)
		///		imap.Search(SearchSince, MakeDateString(new DateTime(2009, 8, 1))
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void AddSearchAnd(int type)
		{
			AddSearchAnd(type, null);
		}
		/// <summary>
		/// メールの検索 OR (文字列有)
		/// </summary>
		/// <param name="type">検索条件
		/// <para>検索条件の設定可能な値は下記の通りです。</para>
		/// <para><see cref="SearchKeyword"/>指定されたキーワードあり</para>
		/// <para><see cref="SearchUnKeyword"/>指定されたキーワードなし</para>
		/// <para><see cref="SearchFrom"/>From ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchTo"/>To ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchCc"/>CC ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchBcc"/>BCC ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchSubject"/>SUBJECT ヘッダに指定の文字列あり</para>
		/// <para><see cref="SearchSentBefore"/>DATE ヘッダが指定された日付以前</para>
		/// <para><see cref="SearchSentOn"/>DATE ヘッダが指定された日付</para>
		/// <para><see cref="SearchSentSince"/>DATE ヘッダが指定された日付以降</para>
		/// <para><see cref="SearchBefore"/>指定された日付以前に到着</para>
		/// <para><see cref="SearchOn"/>指定された日付に到着</para>
		/// <para><see cref="SearchSince"/>指定された日付以降に到着</para>
		/// <para><see cref="SearchHeader"/>指定されたヘッダフィールドに文字列を含む 検索文字列に"ヘッダフィールド 文字列"と指定してください</para>
		/// <para><see cref="SearchBody"/>本文に文字列を含む</para>
		/// <para><see cref="SearchText"/>ヘッダおよび本文に文字列を含む</para>
		/// <para><see cref="SearchLager"/>指定サイズより大きなメール サイズは文字列に変換してください</para>
		/// <para><see cref="SearchSmaller"/>指定サイズより小さなメール サイズは文字列に変換してください</para>
		/// <para><see cref="SearchUid"/>指定 UID のメール</para>
		/// <para><see cref="SearchCommand"/>検索文字列を直接指定</para>
		/// <para><see cref="SearchNot"/>検索条件 NOT (上記の検索条件と同時に(C# では | 、VB.NET では Or)指定する必要があります)</para>
		/// </param>
		/// <param name="text">検索文字列</param>
		/// <remarks>
		///	OR で検索条件を指定します。<see cref="Search"/>で検索を実行します。
		/// <example>
		/// <para>
		///	メールボックス("inbox")内の件名に"テスト"が含まれるか、または差出人(From フィールド)に support@nanshiki.co.jp が含まれるメールを検索する
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.AddSearchOr(SearchSubject, "テスト");
		///			imap.Search(SearchFrom, "support@nanshiki.co.jp");
		///			uint [] list = imap4.GetSearchMailResultList()
		///			foreach(uint no in list)
		///			{
		///				MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.AddSearchOr(SearchSubject, "テスト")
		///		imap.Search(SearchFrom, "support@nanshiki.co.jp")
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void AddSearchOr(int type, string text)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			if(_search_first_flag)
			{
				type |= SearchFirst;
				_search_first_flag = false;
			}
			_err = Imap4SearchMail(_socket, type | SearchOr, text, _useuid);
			if(_err < 0)
			{
				throw new nMailException("Search: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// メールの検索 OR (文字列無)
		/// </summary>
		/// <param name="type">検索条件
		/// <para>検索条件の設定可能な値は下記の通りです。</para>
		/// <para><see cref="SearchAnswered"/>返信済み</para>
		/// <para><see cref="SearchUnAnswered"/>返信済みでない</para>
		/// <para><see cref="SearchDeleted"/>消去マークあり</para>
		/// <para><see cref="SearchUnDeleted"/>消去マークなし</para>
		/// <para><see cref="SearchDraft"/>草稿フラグあり</para>
		/// <para><see cref="SearchUnDraft"/>草稿フラグなし</para>
		/// <para><see cref="SearchFlagged"/>重要性フラグあり</para>
		/// <para><see cref="SearchUnFlagged"/>重要性フラグなし</para>
		/// <para><see cref="SearchRecent"/>到着フラグあり</para>
		/// <para><see cref="SearchUnRecent"/>到着フラグなし</para>
		/// <para><see cref="SearchSeen"/>既読フラグあり</para>
		/// <para><see cref="SearchUnSeen"/>既読フラグなし</para>
		/// <para><see cref="SearchNew"/>到着フラグありかつ既読フラグなし</para>
		/// <para><see cref="SearchAll"/>全てのメール</para>
		/// <para><see cref="SearchNot"/>検索条件 NOT (上記の検索条件と同時に(C# では | 、VB.NET では Or)指定する必要があります)</para>
		/// </param>
		/// <remarks>
		///	OR で検索条件を指定します。<see cref="Search"/>で検索を実行します。
		/// <example>
		/// <para>
		///	メールボックス("inbox")内の既読のメールか、または件名に"テスト"が含まれるメールを検索する
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.AddSearchOr(SearchSeen);
		///			imap.Search(SearchSubject, "テスト");
		///			uint [] list = imap4.GetSearchMailResultList()
		///			foreach(uint no in list)
		///			{
		///				MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.AddSearchOr(SearchSeen)
		///		imap.Search(SearchSubject, "テスト")
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void AddSearchOr(int type)
		{
			AddSearchOr(type, null);
		}

		/// <summary>
		/// メールボックスにメールをアップロードします。
		/// </summary>
		/// <param name="mailbox">アップロードするメールボックス</param>
		/// <param name="header">メールヘッダ
		/// <para>Date,Message-ID,Mime-Version,Content-Type,Content-Transfer-Encode フィールド</para>
		/// <para>は自動的に付加されます。Date フィールドには現在日時が設定されます。</para>
		/// <para>Date および Message-ID フィールドは引数 header に記述があればそちらが優先されます。</para>
		/// </param>
		/// <param name="body">メール本文</param>
		/// <param name="message_flag">メッセージフラグ
		/// <para><see cref="MessageAnswerd"/>返信済みフラグ</para>
		/// <para><see cref="MessageDeleted"/>消去マーク</para>
		/// <para><see cref="MessageDraft"/>草稿フラグ</para>
		/// <para><see cref="MessageFlagged"/>重要性フラグ</para>
		/// <para><see cref="MessageRecent"/>到着フラグ</para>
		/// <para><see cref="MessageSeen"/>既読フラグ</para>
		/// <para>C# は | 、VB.NET では Or で複数指定可能です。</para>
		/// </param>
		/// <param name="path">添付ファイル名</param>
		/// <remarks>
		/// <example>
		/// <para>
		///	送信済メールボックス("sent")にメールを既読フラグ付きでアップロードする。
		/// subject に件名、from に差出人、to に宛先、body にメール本文、path に添付ファイルのパス名が入っているものとする。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		string header;
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			header = "Subject: " + subject + "\r\n";
		///			header += "From: " + from + "\r\n";
		///			header += "To: " + to + "\r\n";
		///			imap.AppendMail("sent", header, body, MessageSeen, path);
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Dim header As String
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		header = "Subject: " + subject + ControlChars.CrLf
		///		header += "From: " + from + ControlChars.CrLf
		///		header += "To: " + to + ControlChars.CrLf
		///		imap.AppendMail("sent", header, body, MessageSeen, path)
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void AppendMail(string mailbox, string header, string body, int message_flag, string path)
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4AppendMail(_socket, mailbox, header, body, path, message_flag, null, AddDateField | AddMessageId);
			if(_err < 0)
			{
				throw new nMailException("AppendMail: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// メールボックスにメールをアップロードします。
		/// </summary>
		/// <param name="mailbox">アップロードするメールボックス</param>
		/// <param name="header">メールヘッダ
		/// <para>Date,Message-ID,Mime-Version,Content-Type,Content-Transfer-Encode フィールド</para>
		/// <para>は自動的に付加されます。Date フィールドには現在日時が設定されます。</para>
		/// <para>Date および Message-ID フィールドは引数 header に記述があればそちらが優先されます。</para>
		/// </param>
		/// <param name="body">メール本文</param>
		/// <param name="message_flag">メッセージフラグ
		/// <para><see cref="MessageAnswerd"/>返信済みフラグ</para>
		/// <para><see cref="MessageDeleted"/>消去マーク</para>
		/// <para><see cref="MessageDraft"/>草稿フラグ</para>
		/// <para><see cref="MessageFlagged"/>重要性フラグ</para>
		/// <para><see cref="MessageRecent"/>到着フラグ</para>
		/// <para><see cref="MessageSeen"/>既読フラグ</para>
		/// <para>C# は | 、VB.NET では Or で複数指定可能です。</para>
		/// </param>
		/// <remarks>
		/// <example>
		/// <para>
		///	送信済メールボックス("sent")にメールを既読フラグ付きでアップロードする。
		/// subject に件名、from に差出人、to に宛先、body にメール本文が入っているものとする。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		string header;
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			header = "Subject: " + subject + "\r\n";
		///			header += "From: " + from + "\r\n";
		///			header += "To: " + to + "\r\n";
		///			imap.AppendMail("sent", header, body, MessageSeen);
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Dim header As String
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		header = "Subject: " + subject + ControlChars.CrLf
		///		header += "From: " + from + ControlChars.CrLf
		///		header += "To: " + to + ControlChars.CrLf
		///		imap.AppendMail("sent", header, body, MessageSeen)
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void AppendMail(string mailbox, string header, string body, int message_flag)
		{
			AppendMail(mailbox, header, body, message_flag, null);
		}

		/// <summary>
		/// 名前空間を取得します。
		/// </summary>
		/// <remarks>
		/// <example>
		/// <para>
		///	名前空間を取得し、表示する。
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		string header;
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.GetNameSpace();
		///			nMail.Imap4.NameSpaceStatus [] list = imap.GetNameSpacePersonalStatusList()
		///			foreach(nMail.Imap4.NameSpaceStatus item in list) {
		///				MessageBox.Show(string.Format("個人名前空間:{0:s} 区切り文字:{1:c}", item.Name, item.Separate));
		///			}
		///			list = imap.GetNameSpaceOtherStatusList()
		///			foreach(nMail.Imap4.NameSpaceStatus item in list) {
		///				MessageBox.Show(string.Format("他人名前空間:{0:s} 区切り文字:{1:c}", item.Name, item.Separate));
		///			}
		///			list = imap.GetNameSpaceSharedStatusList()
		///			foreach(nMail.Imap4.NameSpaceStatus item in list) {
		///				MessageBox.Show(string.Format("共有名前空間:{0:s} 区切り文字:{1:c}", item.Name, item.Separate));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Dim header As String
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.GetNameSpace()
		///		Dim list As nMail.Imap4.NameSpaceStatus() = imap.GetNameSpacePersonalStatusList()
		///		For Each item As nMail.Imap4.NameSpaceStatus In list
		///			MessageBox.Show(String.Format("個人名前空間:{0:s} 区切り文字:{1:c}", item.Name, item.Separate))
		///		Next
		///		list = imap.GetNameSpaceOtherStatusList()
		///		For Each item As nMail.Imap4.NameSpaceStatus In list
		///			MessageBox.Show(String.Format("他人名前空間:{0:s} 区切り文字:{1:c}", item.Name, item.Separate))
		///		Next
		///		list = imap.GetNameSpaceSharedStatusList()
		///		For Each item As nMail.Imap4.NameSpaceStatus In list
		///			MessageBox.Show(String.Format("共有名前空間:{0:s} 区切り文字:{1:c}", item.Name, item.Separate))
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	接続状態ではありません。(<see cref="Connect"/>が成功していないか、呼び出されていません。
		///	</exception>
		///	<exception cref="nMailException">
		///	サーバとの交信中にエラーが発生しました。
		/// <see cref="nMailException.Message"/>にエラーメッセージ、
		/// <see cref="nMailException.ErrorCode"/>にエラーコードが入ります。
		/// </exception>
		public void GetNameSpace()
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4GetNameSpace(_socket);
			if(_err < 0)
			{
				throw new nMailException("GetNameSpace: " + Options.ErrorMessage, _err);
			}
			else
			{
				int [] c1 = { NameSpacePersonalCount, NameSpaceOtherCount, NameSpaceSharedCount };
				int [] c2 = { NameSpacePersonalNo, NameSpaceOtherNo, NameSpaceSharedNo };
				int no, count;
				SetHeaderSize();
				for(int name_no = 0 ; name_no < NameSpaceMax ; name_no++) {
					count = Imap4GetNameSpaceStatus(_socket, c1[name_no], null, null, 0);
					_namespace_list[name_no] = new NameSpaceStatus[count];
					for(no = 0 ; no < count ; no++) {
						StringBuilder separate = new StringBuilder(_header_size);
						StringBuilder name = new StringBuilder(_header_size);
						if(Imap4GetNameSpaceStatus(_socket, c2[name_no] | no, separate, name, _header_size) == Success)
						{
							_namespace_list[name_no][no].Name = name.ToString();
							_namespace_list[name_no][no].Separate = separate[0];
						}
					}
				}
			}
		}

		/// <summary>
		/// 検索用日付指定文字列を作成します。
		/// </summary>
		/// <remarks>
		/// 日付指定文字列を作成します。
		/// <para>検索用 例: "09-Jul-2009"</para>
		/// <example>
		/// <para>
		///	メールボックス("inbox")内の 2009/8/1 に到着したメールを検索する
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.Search(SearchOn, MakeSearchDateString(new DateTime(2009, 8, 1));
		///			uint [] list = imap4.GetSearchMailResultList()
		///			foreach(uint no in list)
		///			{
		///				MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 以降の場合、C# と同様に using が使用できます。
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.Search(SearchOn, MakeSearchDateString(new DateTime(2009, 8, 1))
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("検索結果メール番号 {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("エラー 番号:{0:d} メッセージ:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <returns>作成した日付指定文字列</returns>
		public string MakeSearchDateString(DateTime tm)
		{
			StringBuilder _datestring = new StringBuilder(DateStringSize);
			MakeDateString(_datestring, tm.Year, tm.Month, tm.Day, tm.Hour, tm.Minute, tm.Second, DateImap4);
			return _datestring.ToString();
		}
		/// <summary>
		/// AppenMail用日時指定文字列を作成します。
		/// </summary>
		/// <remarks>
		/// 日時指定文字列を作成します。
		/// <para>AppendMail用 例: "09-Jul-2009 15:10:30"</para>
		/// <para>※今のところ AppendMail でアップロードする際のサーバが内部的に持つ日時指定には</para>
		/// <para>　Imap4 クラスのメソッドでは対応していません。</para>
		/// <para>　Date ヘッダフィールドの値は <see cref="MakeFieldDateString"/> で作成して付加することができます。</para>
		/// </remarks>
		/// <returns>作成した日時指定文字列</returns>
		public string MakeAppendDateString(DateTime tm)
		{
			StringBuilder _datestring = new StringBuilder(DateStringSize);
			MakeDateString(_datestring, tm.Year, tm.Month, tm.Day, tm.Hour, tm.Minute, tm.Second, DateTimeImap4);
			return _datestring.ToString();
		}
		/// <summary>
		/// Date ヘッダフィールド用日時指定文字列を作成します。
		/// </summary>
		/// <remarks>
		/// 日時指定文字列を作成します。
		/// <para>Date ヘッダフィールド用 例: "Thu, 9 Jul 2009 15:10:30 +09:00"</para>
		/// <para><see cref="AppendMail"/> で指定の日時の Date ヘッダフィールドを付加したい場合に使用します。</para>
		/// </remarks>
		/// <returns>作成した日時指定文字列</returns>
		public string MakeFieldDateString(DateTime tm)
		{
			StringBuilder _datestring = new StringBuilder(DateStringSize);
			MakeDateString(_datestring, tm.Year, tm.Month, tm.Day, tm.Hour, tm.Minute, tm.Second, DateTimeSmtp);
			return _datestring.ToString();
		}
		/// <summary>
		/// なにも実行しません
		/// </summary>
		/// <remarks>
		/// サーバとの接続タイムアウト防止等に使用します。
		/// </remarks>
		public void NoOperation()
		{
			if(_socket == (IntPtr)ErrorSocket) {
				throw new InvalidOperationException();
			}
			_err = Imap4NoOperation(_socket);
			if(_err < 0)
			{
				throw new nMailException("NoOperation: " + Options.ErrorMessage, _err);
			}
		}
		/// <summary>
		/// 添付ファイル名の配列を取得します。
		/// </summary>
		/// <returns>添付ファイル名の配列</returns>
		public string[] GetFileNameList()
		{
			if(_filename_list == null)
			{
				return new string[0];
			}
			else
			{
				return (string[])_filename_list.Clone();
			}
		}
		/// <summary>
		/// メールボックス情報の配列を取得します。
		/// </summary>
		/// <returns>メールボックス情報の配列</returns>
		public MailBoxStatus[] GetMailBoxStatusList()
		{
			return (MailBoxStatus[])_mailbox_list.Clone();
		}
		/// <summary>
		/// MIME 構造の配列を取得します。
		/// </summary>
		/// <returns>MIME 構造情報の配列</returns>
		public MimeStructureStatus[] GetMimeStructureStatusList()
		{
			return (MimeStructureStatus[])_mime_list.Clone();
		}
		/// <summary>
		/// 検索結果の配列を取得します。
		/// </summary>
		/// <returns>検索結果の配列</returns>
		public uint[] GetSearchMailResultList()
		{
			return (uint[])_search_list.Clone();
		}
		/// <summary>
		/// 個人名前空間の配列を取得します。
		/// </summary>
		/// <remarks>
		///	<para><see cref="GetNameSpace"/>実行後に呼び出す必要があります。</para>
		/// </remarks>
		/// <returns>個人名前空間情報の配列</returns>
		public NameSpaceStatus[] GetNameSpacePersonalStatusList()
		{
			return (NameSpaceStatus[])_namespace_list[NameSpacePersonal].Clone();
		}
		/// <summary>
		/// 他人名前空間の配列を取得します。
		/// </summary>
		/// <remarks>
		///	<para><see cref="GetNameSpace"/>実行後に呼び出す必要があります。</para>
		/// </remarks>
		/// <returns>他人名前空間情報の配列</returns>
		public NameSpaceStatus[] GetNameSpaceOtherStatusList()
		{
			return (NameSpaceStatus[])_namespace_list[NameSpaceOther].Clone();
		}
		/// <summary>
		/// 共有名前空間の配列を取得します。
		/// </summary>
		/// <remarks>
		///	<para><see cref="GetNameSpace"/>実行後に呼び出す必要があります。</para>
		/// </remarks>
		/// <returns>共有名前空間情報の配列</returns>
		public NameSpaceStatus[] GetNameSpaceSharedStatusList()
		{
			return (NameSpaceStatus[])_namespace_list[NameSpaceShared].Clone();
		}

		/// <summary>
		/// IMAP4 ポート番号です。
		/// </summary>
		/// <value>IMAP4 ポート番号</value>
		public int Port {
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}
		/// <summary>
		/// ソケットハンドルです。
		/// </summary>
		/// <value>ソケットハンドル</value>
		public IntPtr Handle {
			get
			{
				return _socket;
			}
		}
		/// <summary>
		/// メールボックスにあるメール数です。
		/// </summary>
		/// <value>現在選択されているメールボックスにあるメール数</value>
		public int Count {
			get
			{
				return _count;
			}
		}
		/// <summary>
		/// メールのサイズです。
		/// </summary>
		/// <value>メールのサイズ</value>
		public int Size {
			get
			{
				return _size;
			}
		}
		/// <summary>
		/// IMAP4 サーバ名です。
		/// </summary>
		/// <value>IMAP4 サーバ名</value>
		public string HostName
		{
			get
			{
				return _host;
			}
			set
			{
				_host = value;
			}
		}
		/// <summary>
		/// IMAP4 ユーザー名です。
		/// </summary>
		/// <value>IMAP4 ユーザー名</value>
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		/// <summary>
		/// IMAP4 パスワードです。
		/// </summary>
		/// <value>IMAP4 パスワード</value>
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
			}
		}
		/// <summary>
		/// 添付ファイルを保存するフォルダです。
		/// </summary>
		/// <remarks>
		/// null (VB.Net は nothing) の場合保存しません。
		/// </remarks>
		/// <value>添付ファイル保存フォルダ</value>
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}
		/// <summary>
		/// メールの本文です。
		/// </summary>
		/// <value>メール本文</value>
		public string Body
		{
			get
			{
				if(_body == null)
				{
					return "";
				}
				else
				{
					return _body.ToString();
				}
			}
		}
		/// <summary>
		/// メールの件名です。
		/// </summary>
		/// <value>メールの件名</value>
		public string Subject
		{
			get
			{
				if(_subject == null)
				{
					return "";
				}
				else
				{
					return _subject.ToString();
				}
			}
		}
		/// <summary>
		/// メールの送信日時の文字列です。
		/// </summary>
		/// <value>メール送信日時文字列</value>
		public string DateString
		{
			get
			{
				if(_date == null)
				{
					return "";
				}
				else
				{
					return _date.ToString();
				}
			}
		}
		/// <summary>
		/// メールの差出人です。
		/// </summary>
		/// <value>メールの差出人</value>
		public string From
		{
			get
			{
				if(_from == null)
				{
					return "";
				}
				else
				{
					return _from.ToString();
				}
			}
		}
		/// <summary>
		/// メールのヘッダです。
		/// </summary>
		/// <value>メールのヘッダ</value>
		public string Header
		{
			get
			{
				if(_header == null)
				{
					return "";
				}
				else
				{
					return _header.ToString();
				}
			}
			set
			{
				_header = new StringBuilder(value);
			}
		}
		/// <summary>
		/// 添付ファイル名です。
		/// </summary>
		/// <remarks>
		/// 複数の添付ファイルがある場合、"," で区切られて格納されます。
		/// <see cref="Options.SplitChar"/>で区切り文字を変更できます。
		/// </remarks>
		/// <value>添付ファイル名</value>
		public string FileName
		{
			get
			{
				if(_filename == null)
				{
					return "";
				}
				else
				{
					return _filename.ToString();
				}
			}
		}
		/// <summary>
		/// 添付ファイル名の配列です。
		/// </summary>
		///	<remarks>
		/// このプロパティは互換性のために残してあります。
		///	<see cref="GetFileNameList"/>で配列を取得して使用するようにしてください。
		///	</remarks>
		/// <value>添付ファイル名の配列</value>
		public string[] FileNameList
		{
			get
			{
				return _filename_list;
			}
		}
		/// <summary>
		/// メールボックス情報の配列
		/// </summary>
		///	<remarks>
		///	<see cref="GetMailBoxStatusList"/>で配列を取得して使用するのを推薦します。
		///	</remarks>
		/// <value>メールボックス情報の配列</value>
		public MailBoxStatus[] MailBoxStatusList
		{
			get
			{
				return _mailbox_list;
			}
		}
		/// <summary>
		/// MIME 構造情報の配列
		/// </summary>
		///	<remarks>
		///	<see cref="GetMimeStructureStatusList"/>で配列を取得して使用するのを推薦します。
		///	</remarks>
		/// <value>MIME 構造情報の配列</value>
		public MimeStructureStatus[] MimeStructureStatusList
		{
			get
			{
				return _mime_list;
			}
		}
		/// <summary>
		/// 検索結果の配列
		/// </summary>
		///	<remarks>
		///	<see cref="GetSearchMailResultList"/>で配列を取得して使用するのを推薦します。
		///	</remarks>
		/// <value>検索結果の配列</value>
		public uint[] SearchMailResultList
		{
			get
			{
				return _search_list;
			}
		}
		/// <summary>
		/// 個人名前空間情報の配列
		/// </summary>
		///	<remarks>
		///	<see cref="GetNameSpacePersonalStatusList"/>で配列を取得して使用するのを推薦します。
		///	</remarks>
		/// <value>個人名前空間情報の配列</value>
		public NameSpaceStatus[] NameSpacePersonalStatusList
		{
			get
			{
				return _namespace_list[NameSpacePersonal];
			}
		}
		/// <summary>
		/// 他人名前空間情報の配列
		/// </summary>
		///	<remarks>
		///	<see cref="GetNameSpaceOtherStatusList"/>で配列を取得して使用するのを推薦します。
		///	</remarks>
		/// <value>個人名前空間情報の配列</value>
		public NameSpaceStatus[] NameSpaceOtherStatusList
		{
			get
			{
				return _namespace_list[NameSpaceOther];
			}
		}
		/// <summary>
		/// 共有名前空間情報の配列
		/// </summary>
		///	<remarks>
		///	<see cref="GetNameSpaceSharedStatusList"/>で配列を取得して使用するのを推薦します。
		///	</remarks>
		/// <value>個人名前空間情報の配列</value>
		public NameSpaceStatus[] NameSpaceSharedStatusList
		{
			get
			{
				return _namespace_list[NameSpaceShared];
			}
		}
		/// <summary>
		/// ヘッダのフィールド名です。
		/// </summary>
		/// <value>ヘッダのフィールド名</value>
		public string FieldName
		{
			get
			{
				if(_field_name == null)
				{
					return "";
				}
				else
				{
					return _field_name;
				}
			}
		}
		/// <summary>
		/// ヘッダフィールドの内容です。
		/// </summary>
		/// <value>ヘッダフィールドの内容</value>
		public string Field
		{
			get
			{
				if(_field == null)
				{
					return "";
				}
				else
				{
					return _field.ToString();
				}
			}
		}
		/// <summary>
		/// メールの UID です。
		/// </summary>
		/// <value>メール UID</value>
		public uint Uid
		{
			get
			{
				return _uid;
			}
		}
		/// <summary>
		/// IMAP4 認証方法です。
		/// </summary>
		/// <remarks>
		/// <para><see cref="AuthLogin"/> で LOGIN を使用します。</para>
		/// <para><see cref="AuthPlain"/> で PLAIN を使用します。</para>
		/// <para><see cref="AuthCramMd5"/> CRAM MD5 を使用します。</para>
		/// <para><see cref="AuthDigestMd5"/> DIGEST MD5 を使用します。</para>
		/// <para>C# は | 、Visual Basic では or で複数設定可能です。</para>
		/// <para>DIGEST MD5->CRAM MD5→PLAIN→LOGIN の順番で認証を試みます。</para>
		///	<para>デフォルト値は AuthLogin です。</para>
		/// </remarks>
		/// <value>IMAP4 の認証方法</value>
		public int AuthMode
		{
			get
			{
				return _mode;
			}
			set
			{
				_mode = value;
			}
		}
		/// <summary>
		/// メール拡張機能指定フラグです。
		/// </summary>
		/// <remarks>
		/// <para><see cref="SuspendAttachmentFile"/>添付ファイル受信で一時休止ありの一回目に指定します。</para>
		/// <para><see cref="SuspendNext"/>添付ファイル受信で一時休止ありの二回目以降に指定します。</para>
		/// </remarks>
		public int Flag
		{
			get
			{
				return _flag;
			}
			set
			{
				_flag = value;
			}
		}
		/// <summary>
		/// MIME パート取得もしくは保存時の設定フラグです。
		/// </summary>
		/// <remarks>
		/// <para><see cref="MimePeek"/>既読フラグを立てません。</para>
		/// <para><see cref="MimeNoDecode"/>デコードしません。</para>
		/// </remarks>
		public int MimeFlag
		{
			get
			{
				return _mime_flag;
			}
			set
			{
				_mime_flag = value;
			}
		}
		/// <summary>
		/// メールメッセージのフラグです。
		/// </summary>
		/// <remarks>
		/// <para><see cref="MessageAnswerd"/>返信済みフラグが付いています。</para>
		/// <para><see cref="MessageDeleted"/>消去マークが付いています。</para>
		/// <para><see cref="MessageDraft"/>草稿フラグが付いています。</para>
		/// <para><see cref="MessageFlagged"/>重要性フラグが付いています。</para>
		/// <para><see cref="MessageRecent"/>到着フラグが付いています。</para>
		/// <para><see cref="MessageSeen"/>既読フラグが付いています。</para>
		/// </remarks>
		public int MessageFlag
		{
			get
			{
				return _message_flag;
			}
		}
		/// <summary>
		/// メールを指定する際に UID を使用するかどうかのフラグです。
		/// </summary>
		public bool UseUid
		{
			get
			{
				return _useuid_flag;
			}
			set
			{
				_useuid_flag = value;
				if(_useuid_flag) {
					_useuid = UseUidValue;
				} else {
					_useuid = 0;
				}
			}
		}
		/// <summary>
		/// エラー番号です。
		/// </summary>
		/// <value>エラー番号</value>
		public int ErrorCode
		{
			get
			{
				return _err;
			}
		}
		/// <summary>
		/// text/html パートを保存したファイルの名前です。
		/// </summary>
		/// <value>ファイル名</value>
		public string HtmlFile
		{
			get
			{
				if(_html_file == null)
				{
					return "";
				}
				else
				{
					return _html_file.ToString();
				}
			}
		}
		/// <summary>
		/// SSL 設定フラグです。
		/// </summary>
		/// <value>SSL 設定フラグ</value>
		public int SSL {
			get
			{
				return _ssl;
			}
			set
			{
				_ssl = value;
			}
		}
		/// <summary>
		/// SSL クライアント証明書名です。
		/// </summary>
		/// <remarks>
		/// null の場合指定しません。
		/// </remarks>
		/// <value>SSL クライアント証明書名</value>
		public string CertName
		{
			get
			{
				return _cert_name;
			}
			set
			{
				_cert_name = value;
			}
		}
		/// <summary>
		/// message/rfc822 パートを保存したファイルの名前です。
		/// </summary>
		/// <value>ファイル名</value>
		public string Rfc822File
		{
			get
			{
				if(_rfc822_file == null)
				{
					return "";
				}
				else
				{
					return _rfc822_file.ToString();
				}
			}
		}
	}
}

/**
	DLL 宣言例

	[DllImport("nMail.DLL", EntryPoint="NMailInitializeWinSock")]
	protected static extern bool NMailInitializeWinSock();

	[DllImport("nMail.DLL", EntryPoint="NMailEndWinSock")]
	protected static extern bool NMailEndWinSock();

	[DllImport("nMail.DLL", EntryPoint="NMailPop3Connect", CharSet=CharSet.Auto)]
	protected static extern int NMailPop3Connect(string Host);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3ConnectPortNo", CharSet=CharSet.Auto)]
	protected static extern int NMailPop3ConnectPortNo(string Host, int Port);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3Authenticate", CharSet=CharSet.Auto)]
	protected static extern int NMailPop3Authenticate(int Socket, string Id, string Pass, bool APopFlag);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3Close")]
	protected static extern int NMailPop3Close(int Socket);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3GetMailStatus", CharSet=CharSet.Auto)]
	protected static extern int NMailPop3GetMailStatus(int Socket, int No, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, bool SizeFlag);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3GetMailSize")]
	protected static extern int NMailPop3GetMailSize(int Socket, int No);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3GetMail", CharSet=CharSet.Auto)]
	protected static extern int NMailPop3GetMail(int Socket, int No, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, StringBuilder Body, string Path, StringBuilder FileName);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3GetMailPartial", CharSet=CharSet.Auto)]
	protected static extern int NMailPop3GetMailPartial(int Socket, int No, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, StringBuilder Body, string Path, StringBuilder FileName);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3GetMailEx", CharSet=CharSet.Auto)]
	protected static extern int NMailPop3GetMailEx(int Socket, int No, StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, StringBuilder Body, string Path, StringBuilder FileName, BYTE Temp[], int Flag);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3DeleteMail")]
	protected static extern int NMailPop3DeleteMail(int Socket, int No);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3GetAttachmentFileStatus"), CHarSet=CharSet.Auto]
	protected static extern int NMailPop3GetAttachmentFileStatus(int Socket, int No, StringBuilder Id, int Max);

	[DllImport("nMail.DLL", EntryPoint="NMailPop3GetUidl"), CHarSet=CharSet.Auto]
	protected static extern int NMailPop3GetUidl(int Socket, int No, StringBuilder Id, int Max);

	[DllImport("nMail.DLL", EntryPoint="NMailSmtpSendMail", CharSet=CharSet.Auto)]
	protected static extern int NMailSmtpSendMail(String HostName, String To, String Cc, String Bcc, String From, String Subject, String Body, String Header, String Path, int Flag);

	[DllImport("nMail.DLL", EntryPoint="NMailSmtpSendMailPortNo", CharSet=CharSet.Auto)]
	protected static extern int NMailSmtpSendMailPortNo(String HostName, String To, String Cc, String Bcc, String From, String Subject, String Body, String Header, String Path, int Flag, int PortNo);

	[DllImport("nMail.DLL", EntryPoint="NMailSmtpConnect", CharSet=CharSet.Auto)]
	protected static extern int NMailSmtpConnect(string Host);

	[DllImport("nMail.DLL", EntryPoint="NMailSmtpConnectPortNo", CharSet=CharSet.Auto)]
	protected static extern int NMailSmtpConnectPortNo(string Host, int PortNo);

	[DllImport("nMail.DLL", EntryPoint="NMailSmtpAuthenticate", CharSet=CharSet.Auto)]
	protected static extern int NMailSmtpAuthenticate(int Socket, string HostName, string Id, string Pass, int Mode);

	[DllImport("nMail.DLL", EntryPoint="NMailSmtpSendMailEx", CharSet=CharSet.Auto)]
	protected static extern int NMailSmtpSendMailEx(int Socket, string To, string Cc, string Bcc, string From, string Subject, String Body, String Header, String Path, BYTE Temp[], int Flag);

	[DllImport("nMail.DLL", EntryPoint="NMailSmtpClose")]
	protected static extern bool NMailSmtpClose(int Socket);

	[DllImport("nMail.DLL", EntryPoint="NMailGetHeaderField"), CharSet=CharSet.Auto]
	protected static extern int NMailGetHeaderField(StringBuilder Field, string Header, string Name, int Size);

	[DllImport("nMail.DLL", EntryPoint="NMailDecodeHeaderField"), CharSet=CharSet.Auto]
	protected static extern int NMailDecodeHeaderField(StringBuilder Destination, string Source, int Size);

	[DllImport("nMail.DLL", EntryPoint="NMailAttachmentFileFirst"), CharSet=CharSet.Auto]
	protected static extern int NMailAttachmentFileFirst(BYTE Temp[], StringBuilder Subject, StringBuilder Date, StringBuilder From, StringBuilder Header, StringBuilder Body, string Path, StringBuilder FileName, string FirstHeader, string FirstBody);

	[DllImport("nMail.DLL", EntryPoint="NMailAttachmentFileNext"), CharSet=CharSet.Auto]
	protected static extern int NMailAttachmentFileNext(BYTE Temp[], string NextHeader, string NextBody);

	[DllImport("nMail.DLL", EntryPoint="NMailAttachmentFileClose")]
	protected static extern bool NMailAttachmentFileClose(BYTE Temp[]);

	[DllImport("nMail.DLL", EntryPoint="NMailAttachmentFileStatus"), CharSet=CharSet.Auto]
	protected static extern int NMailAttachmentFileStatus(string Header, StringBuilder Id, int Size);

	[DllImport("nMail.DLL", EntryPoint="NMailGetVersion")]
	protected static extern int NMailGetVersion();

	[DllImport("nMail.DLL", EntryPoint="NMailGetOption")]
	protected static extern int NMailGetOption(int Option);

	[DllImport("nMail.DLL", EntryPoint="NMailSetOption")]
	protected static extern int NMailSetOption(int Option, int Value);

	[DllImport("nMail.DLL", EntryPoint="NMailGetMessage", CharSet=CharSet.Auto)]
	protected static extern void NMailGetMessage(int Type, StringBuilder Message, int Size);

	[DllImport("nMail.DLL", EntryPoint="NMailGetSuspendNumber", CharSet=CharSet.Auto)]
	protected static extern void NMailGetSuspendNumber(string Path);
**/

