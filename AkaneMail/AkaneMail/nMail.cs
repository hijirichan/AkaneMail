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
	/// nMail.DLL ����G���[���Ԃ��Ă����ꍇ�ɔ��������O
	/// </summary>
	public class nMailException: ApplicationException, ISerializable
	{
		int _error_code;
		/// <summary>
		/// �G���[���b�Z�[�W�ƃG���[�R�[�h���w�肵�āA<c>nMailException</c>�N���X�̐V�����C���X�^���X�����������܂��B
		/// </summary>
		/// <param name="message"></param>
		/// <param name="error_code"></param>
		public nMailException(string message, int error_code): base(message)
		{
			this._error_code = error_code;
		}
		/// <summary>
		/// �V���A���������f�[�^���w�肵�āA<c>nMailException</c>�N���X�̐V�����C���X�^���X�����������܂��B
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public nMailException(SerializationInfo info, StreamingContext context): base(info, context)
		{
			_error_code = info.GetInt32("ErrorCode");
		}
		/// <summary>
		/// �G���[�R�[�h�ł��B���l�� nMail.chm �� Q��A �̃G���[�R�[�h�ꗗ���Q�Ƃ��Ă݂Ă��������B
		/// </summary>
		public int ErrorCode
		{
			get
			{
				return _error_code;
			}
		}
		/// <summary>
		/// �T�[�o�[����Ԃ��ꂽ�G���[���b�Z�[�W�ł��B
		/// </summary>
		public override string Message
		{
			get
			{
				return base.Message;
			}
		}
		/// <summary>
		/// ��O�Ɋւ�������g�p���� SerializationInfo ��ݒ肵�܂��B
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
	/// Winsock �֘A�N���X
	/// </summary>
	public class Winsock
	{
		/// <summary>
		/// <c>Winsock</c>�N���X�̐V�K�C���X�^���X�����������܂��B
		/// </summary>
		public Winsock()
		{
		}
		/// <summary>
		/// Winsock �����������܂��B
		/// </summary>
		[DllImport("nMail.DLL", EntryPoint="NMailInitializeWinSock")]
		public static extern bool Initialize();

		/// <summary>
		/// Winsock �̎g�p���I�����܂��B
		/// </summary>
		[DllImport("nMail.DLL", EntryPoint="NMailEndWinSock")]
		public static extern bool Done();
	}
	/// <summary>
	/// POP3���[����M�N���X
	/// </summary>
	/// <remarks>
	/// <example>
	/// <para>
	/// �w��̃��[���ԍ�(�ϐ���:no)���擾���A�����Ɩ{����\������B
	/// ��ق� Attachment �N���X�œY�t�t�@�C����W�J����ꍇ���� text/html �p�[�g���t�@�C���ɕۑ��������ꍇ�A
	/// <see cref="Options.DisableDecodeBodyText()"/> �������� <see cref="Options.DisableDecodeBodyAll()"/>���Ă�ł���<see cref="GetMail"/>�Ŏ擾�����w�b�_����і{���f�[�^���g�p����K�v������܂��B
	/// <code lang="cs">
	///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
	///		try {
	///			pop.Connect();
	///			pop.Authenticate("pop3_id", "password");
	///			pop.GetMail(no);
	///			MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}\r\n{2:s}", no, pop.Subject, pop.Body));
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
	/// Try
	/// 	pop.Connect()
	/// 	pop.Authenticate("pop3_id", "password")
	///		pop.GetMail(no)
	///		MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}" + ControlChars.CrLf + "{2:s}", no, pop.Subject, pop.Body))
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		pop.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// SSL Version 3 ���g�p���A�w��̃��[���ԍ�(�ϐ���:no)���擾���A�����Ɩ{����\������B
	/// <code lang="cs">
	///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
	///		try {
	///			pop.SSL = nMail.Pop3.SSL3;
	///			pop.Connect(nMail.Pop3.StandardSslPortNo);		// over SSL/TLS �̃|�[�g�ԍ����w�肵�Đڑ�
	///			pop.Authenticate("pop3_id", "password");
	///			pop.GetMail(no);
	///			MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}\r\n{2:s}", no, pop.Subject, pop.Body));
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
	/// Try
	/// 	pop.SSL = nMail.Pop3.SSL3
	/// 	pop.Connect(nMail.Pop3.StandardSslPortNo)		' over SSL/TLS �̃|�[�g�ԍ����w�肵�Đڑ�
	/// 	pop.Authenticate("pop3_id", "password")
	///		pop.GetMail(no)
	///		MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}" + ControlChars.CrLf + "{2:s}", no, pop.Subject, pop.Body))
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		pop.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// �w��̃��[���ԍ�(�ϐ���:no)���擾���A�����Ɩ{����\������B�Y�t�t�@�C���� z:\temp �ɕۑ�����B
	/// text/html �p�[�g���t�@�C���ɕۑ�����ꍇ�A<see cref="GetMail"/> �̑O�� <see cref="Options.EnableSaveHtmlFile()"/> ���Ă�ł����K�v������܂��B
	/// �ۑ����� text/html �p�[�g�̃t�@�C������ <see cref="HtmlFile"/> �Ŏ擾�ł��܂��B
	/// <code lang="cs">
	///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
	///		try {
	///			pop.Connect();
	///			pop.Authenticate("pop3_id", "password");
	///			pop.Path = @"z:\temp";
	///			pop.GetMail(no);
	///			MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}\r\n{2:s}", no, pop.Subject, pop.Body));
	///			string [] file_list = pop.GetFileNameList();
	///			if(file_list.Length == 0)
	///			{
	///				MessageBox.Show("�t�@�C���͂���܂���");
	///			}
	///			else
	///			{
	///				foreach(string name in file_list)
	///				{
	///					MessageBox.Show(String.Format("�Y�t�t�@�C����:{0:s}", name));
	///				}
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
	/// Try
	/// 	pop.Connect()
	/// 	pop.Authenticate("pop3_id", "password")
	///		pop.Path = "z:\temp"
	///		pop.GetMail(no)
	///		MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}" + ControlChars.CrLf + "{2:s}", no, pop.Subject, pop.Body))
	///		Dim file_list As String() = pop.GetFileNameList()
	///		If file_list.Length = 0 Then
	///			MessageBox.Show("�t�@�C���͂���܂���")
	///		Else
	///			For Each name As String In file_list
	///				MessageBox.Show(String.Format("�Y�t�t�@�C����:{0:s}", name))
	///			Next fno
	///		End If
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		pop.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	///	�ꎞ�x�~�@�\���g���Ďw��̃��[���ԍ�(�ϐ���:no)���擾���A�����Ɩ{����\������B�Y�t�t�@�C���� z:\temp �ɕۑ�����B
	/// <code lang="cs">
	///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
	///		try {
	///			pop.Connect();
	///			pop.Authenticate("pop3_id", "password");
	///			// ���������̋x�~�񐔂𓾂�
	///			int count = pop.GetSize(no) / (nMail.Options.SuspendSize * 1024) + 1;
	///			pop.Path = @"z:\temp";
	///			pop.GetMail(no);
	///			pop.Flag = nMail.Pop3.SuspendAttachmentFile;
	///			pop.GetMail(no);
	///			pop.Flag = nMail.Pop3.SuspendNext;
	///			while(pop.ErrorCode == nMail.Pop3.ErrorSuspendAttachmentFile)
	///			{
	///				pop.GetMail(no);
	///				// �v���O���X�o�[��i�߂铙�̏���
	///				Application.DoEvents();
	///			}
	///			MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}\r\n{2:s}", no, pop.Subject, pop.Body));
	///			string [] file_list = pop.GetFileNameList();
	///			if(file_list.Length == 0)
	///			{
	///				MessageBox.Show("�t�@�C���͂���܂���");
	///			}
	///			else
	///			{
	///				foreach(string name in file_list)
	///				{
	///					MessageBox.Show(String.Format("�Y�t�t�@�C����:{0:s}", name));
	///				}
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	///	Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
	///	Try
	///		Dim count As Integer
	///
	///		pop.Connect()
	///		pop.Authenticate("pop3_id", "password")
	///		' ���������̋x�~�񐔂𓾂�
	///		count = pop.GetSize(no) \ (nMail.Options.SuspendSize * 1024) + 1
	///		pop.Path = "z:\temp"
	///		pop.Flag = nMail.Pop3.SuspendAttachmentFile
	///		pop.GetMail(no)
	///		pop.Flag = nMail.Pop3.SuspendNext
	///		Do While pop.ErrorCode = nMail.Pop3.ErrorSuspendAttachmentFile
	///			pop.GetMail(no)
	///			' �v���O���X�o�[��i�߂铙�̏���
	///			Application.DoEvents()
	///		Loop
	///		MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}" + ControlChars.CrLf + "{2:s}", no, pop.Subject, pop.Body))
	///		Dim file_list As String() = pop.GetFileNameList()
	///		If file_list.Length = 0 Then
	///			MessageBox.Show("�t�@�C���͂���܂���")
	///		Else
	///			For Each name As String In file_list
	///				MessageBox.Show(String.Format("�Y�t�t�@�C����:{0:s}", name))
	///			Next fno
	///		End If
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
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
		/// �w�b�_�̈�̃T�C�Y�ł��B
		/// </summary>
		protected const int HeaderSize = 32768;
		/// <summary>
		/// �p�X������̃T�C�Y�ł��B
		/// </summary>
		protected const int MaxPath = 32768;
		/// <summary>
		/// �g���@�\�p�o�b�t�@�T�C�Y�ł��B
		/// </summary>
		protected const int AttachmentTempSize = 400;
		/// <summary>
		/// �\�P�b�g�G���[�������͖��ڑ���Ԃł��B�l�� -1 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Connect"/>���Ăяo������<see cref="GetMail"/>�����A</para>
		/// <para>�Ăяo������A�Ȃ�炩�̗��R�Őڑ����ؒf�����Ƃ��̃G���[�ƂȂ�܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorSocket = -1;
		/// <summary>
		/// �F�؃G���[�ł��B�l�� -2 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Authenticate"/>�Ăяo���ŔF�؂Ɏ��s�����ꍇ</para>
		/// <para>���̃G���[�ƂȂ�܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorAuthenticate = -2;
		/// <summary>
		/// �ԍ��Ŏw�肳�ꂽ���[�������݂��܂���B�l�� -3 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>�Ŏw�肵���ԍ��̃��[�������݂��Ȃ��Ƃ��̃G���[�ƂȂ�܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorInvalidNo = -3;
		/// <summary>
		/// �^�C���A�E�g�G���[�ł��B�l�� -4 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Options.Timeout"/>�Ŏw�肵���l��蒷�����ԃT�[�o���牞����</para>
		/// <para>�Ȃ��ꍇ�A���̃G���[���������܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorTimeout = -4;
		/// <summary>
		/// �Y�t�t�@�C�����J���܂���B�l�� -5 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Path"/>�Ŏw�肵���t�H���_�ɓY�t�t�@�C�����������߂Ȃ�</para>
		/// <para>�Ȃ��ꍇ�A���̃G���[���������܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorFileOpen = -5;
		/// <summary>
		/// �����t�@�C����������Ă��܂���B�l�� -6 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Flag"/>��<see cref="PartialAttachmentFile"/> ��ݒ肵��</para>
		/// <para><see cref="GetMail"/>���Ăяo�����ۂɁA�T�[�o��ɕ������ꂽ���[�������ׂ�</para>
		/// <para>�����Ă��Ȃ��ꍇ�ɂ��̃G���[���������܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorPartial = -6;
		/// <summary>
		/// �Y�t�t�@�C���Ɠ����̃t�@�C�����t�H���_�ɑ��݂��܂��B�l�� -7 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="nMail.Options.AlreadyFile"/> �� <see cref="nMail.Options.FileAlreadyError"/></para>
		/// <para>��ݒ肵�A<see cref="Path"/>�Ŏw�肵���t�H���_�ɓY�t�t�@�C���Ɠ������O�̃t�@�C����</para>
		/// <para>����ꍇ�ɂ��̃G���[���������܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorFileAlready = -7;
		/// <summary>
		/// �������m�ۃG���[�ł��B�l�� -9 �ł��B
		/// </summary>
		/// <remarks>
		/// �����ŕ����R�[�h��ϊ����邽�߂̃��������m�ۂł��܂���ł����B
		/// </remarks>
		public const int ErrorMemory = -9;
		/// <summary>
		/// ���̑��̃G���[�ł��B�l�� -10 �ł��B
		/// </summary>
		public const int ErrorEtc = -10;
		/// <summary>
		/// �Y�t�t�@�C����M���ł܂��c�肪�����Ԃł��B�l�� -20 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Flag"/>��<see cref="SuspendAttachmentFile"/>�܂���</para>
		/// <para><see cref="SuspendNext"/>���w�肵<see cref="GetMail"/></para>
		/// <para>���Ăяo�����ꍇ�A�܂����[���̎c�肪����ꍇ�A<see cref="nMailException.ErrorCode"/></para>
		/// <para>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorSuspendAttachmentFile = -20;
		/// <summary>
		/// �Y�t�t�@�C���͑��݂��܂���B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetAttachmentFileStatus"/> ���s��A�Y�t�t�@�C�������݂��Ȃ��ꍇ��</para>
		/// <para><see cref="PartNo"/> �ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int NoAttachmentFile = -1;
		/// <summary>
		/// �Y�t�t�@�C���͑��݂��܂����A��������Ă��܂���B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetAttachmentFileStatus"/> ���s��A�Y�t�t�@�C�������݂��Ȃ��ꍇ��</para>
		/// <para><see cref="PartNo"/> �ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int AttachmentFile = 0;
		/// <summary>
		/// ���s�ɐ������܂����B�l�� 1 �ł��B
		/// </summary>
		/// <para>�e�탁�\�b�h��������Ԃ����ꍇ�A<see cref="nMailException.ErrorCode"/>�ɐݒ肳���l�ł��B</para>
		public const int Success = 1;
		/// <summary>
		/// �������ꂽ�Y�t�t�@�C�����擾���܂��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>�̑O�ɁA<see cref="Flag"/>�ɐݒ肵�Ă����܂��B</para>
		/// </remarks>
		public const int PartialAttachmentFile = 1;
		/// <summary>
		/// ���[���{���̂ݎ擾���܂��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>�̑O�ɁA<see cref="Flag"/>�ɐݒ肵�Ă����܂��B</para>
		/// </remarks>
		public const int TextOnly = 2;
		/// <summary>
		/// �Y�t�t�@�C����M�ňꎞ�x�~����̈��ڂɎw�肵�܂��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>�̑O�ɁA<see cref="Flag"/>�ɐݒ肵�Ă����܂��B</para>
		/// </remarks>
		public const int SuspendAttachmentFile = 4;
		/// <summary>
		/// �Y�t�t�@�C����M�ňꎞ�x�~����̓��ڈȍ~�Ɏw�肵�܂��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>�̑O�ɁA<see cref="Flag"/>�ɐݒ肵�Ă����܂��B</para>
		/// </remarks>
		public const int SuspendNext = 8;
		/// <summary>
		/// UIDL �����ׂĎ擾���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="GetUidl"/>�Ŏw�肷�邱�Ƃɂ���đS�Ă� UIDL ���擾�ł��܂��B
		/// </remarks>
		public const int UidlAll = 0;
		/// <summary>
		/// SSLv3 ���g�p���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int SSL3 = 0x1000;
		/// <summary>
		/// TSLv1 ���g�p���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int TLS1 = 0x2000;
		/// <summary>
		/// STARTTLS ���g�p���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int STARTTLS = 0x4000;
		/// <summary>
		/// �T�[�o�ؖ����������؂�ł��G���[�ɂ��܂���B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int IgnoreNotTimeValid = 0x0800;
		/// <summary>
		/// ���[�g�ؖ����������ł��G���[�ɂ��܂���B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int AllowUnknownCA = 0x0400;
		/// <summary>
		/// POP3 �̕W���|�[�g�ԍ��ł�
		/// </summary>
		public const int StandardPortNo = 110;
		/// <summary>
		/// POP3 over SSL �̃|�[�g�ԍ��ł�
		/// </summary>
		public const int StandardSslPortNo = 995;

		/// <summary>
		/// POP3 �|�[�g�ԍ��ł��B
		/// </summary>
		protected int _port = 110;
		/// <summary>
		/// �\�P�b�g�n���h���ł��B
		/// </summary>
		protected IntPtr _socket = (IntPtr)ErrorSocket;
		/// <summary>
		/// ���[�����ł��B
		/// </summary>
		protected int _count = -1;
		/// <summary>
		/// ���[���T�C�Y�ł��B
		/// </summary>
		protected int _size = -1;
		/// <summary>
		/// �w�b�_�[�T�C�Y�ł��B
		/// </summary>
		protected int _header_size = -1;
		/// <summary>
		/// �{���̃T�C�Y�ł��B
		/// </summary>
		protected int _body_size = -1;
		/// <summary>
		/// ���[����M���̐ݒ�p�t���O�ł��B
		/// </summary>
		protected int _flag = 0;
		/// <summary>
		/// �����t�@�C���ԍ��ł��B
		/// </summary>
		protected int _part_no = -1;
		/// <summary>
		/// �G���[�ԍ��ł��B
		/// </summary>
		protected int _err = 0;
		/// <summary>
		/// APOP ���g�p���邩�ǂ����̃t���O�ł��B
		/// </summary>
		protected bool _apop = false;
		/// <summary>
		/// POP3 �T�[�o���ł��B
		/// </summary>
		protected string _host = "";
		/// <summary>
		/// POP3 ���[�U�[���ł��B
		/// </summary>
		protected string _id = "";
		/// <summary>
		/// POP3 �p�X���[�h�ł��B
		/// </summary>
		protected string _password = "";
		/// <summary>
		/// �Y�t�t�@�C���ۑ��p�̃p�X�ł��B
		/// </summary>
		protected string _path = null;
		/// <summary>
		/// �w�b�_�t�B�[���h���ł��B
		/// </summary>
		protected string _field_name = "";
		/// <summary>
		/// �{���i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _body = null;
		/// <summary>
		/// �����i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _subject = null;
		/// <summary>
		/// ���t������ۑ��o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _date = null;
		/// <summary>
		/// ���o�l�i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _from = null;
		/// <summary>
		/// �w�b�_�i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _header = null;
		/// <summary>
		/// �Y�t�t�@�C�����i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _filename = null;
		/// <summary>
		/// �Y�t�t�@�C�����̃��X�g�ł��B
		/// </summary>
		protected string[] _filename_list = null;
		/// <summary>
		/// �w�b�_�t�B�[���h���e�i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _field = null;
		/// <summary>
		/// �����t�@�C���� ID �ł��B
		/// </summary>
		protected StringBuilder _part_id = null;
		/// <summary>
		/// UIDL �i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _uidl = null;
		/// <summary>
		/// �g���@�\�p�o�b�t�@�ł��B
		/// </summary>
		protected byte[] _temp = null;
		/// <summary>
		/// Dispose �������s�������ǂ����̃t���O�ł��B
		/// </summary>
		private bool _disposed = false;
		/// <summary>
		/// text/html �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		protected StringBuilder _html_file = null;
		/// <summary>
		/// SSL �ݒ�t���O�ł��B
		/// </summary>
		protected int _ssl = 0;
		/// <summary>
		/// SSL �N���C�A���g�ؖ������ł��B
		/// </summary>
		protected string _cert_name = null;
		/// <summary>
		/// message/rfc822 �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		protected StringBuilder _rfc822_file = null;

		/// <summary>
		/// <c>Pop3</c>�N���X�̐V�K�C���X�^���X�����������܂��B
		/// </summary>
		public Pop3()
		{
			Init();
		}
		/// <summary>
		/// <c>Pop3</c>�N���X�̐V�K�C���X�^���X�����������܂��B
		/// <param name="host_name">POP3 �T�[�o�[��</param>
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
		/// <see cref="Pop3"/>�ɂ���Ďg�p����Ă��邷�ׂẴ��\�[�X��������܂��B
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// <see cref="Pop3"/>�ɂ���Ďg�p����Ă��邷�ׂẴ��\�[�X��������܂��B
		/// </summary>
		/// <param name="disposing">
		/// �}�l�[�W���\�[�X�ƃA���}�l�[�W���\�[�X�̗������������ꍇ��<c>true</c>�B
		/// �A���}�l�[�W���\�[�X�������������ꍇ��<c>false</c>�B
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
		/// �����������ł��B
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
		/// �w�b�_�i�[�p�o�b�t�@�̃T�C�Y�����肵�܂��B
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
		/// POP3 �T�[�o�ɐڑ����܂��B
		/// </summary>
		/// <remarks>
		/// POP3 �T�[�o�ɐڑ����܂��B
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	POP �T�[�o�[�Ƃ̐ڑ��Ɏ��s���܂����B
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// POP3 �T�[�o�ɐڑ����܂��B
		/// </summary>
		/// <param name="host_name">POP3 �T�[�o�[��</param>
		/// <remarks>
		/// POP3 �T�[�o�ɐڑ����܂��B
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	POP �T�[�o�Ƃ̐ڑ��Ɏ��s���܂����B
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Connect(string host_name)
		{
			_host = host_name;
			Connect();
		}
		/// <summary>
		/// POP3 �T�[�o�ɐڑ����܂��B
		/// </summary>
		/// <param name="host_name">POP3 �T�[�o��</param>
		/// <param name="port_no">�|�[�g�ԍ�</param>
		/// <remarks>
		/// POP3 �T�[�o�ɐڑ����܂��B
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	POP �T�[�o�Ƃ̐ڑ��Ɏ��s���܂����B
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Connect(string host_name, int port_no)
		{
			_host = host_name;
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// POP3 �T�[�o�ɐڑ����܂��B
		/// </summary>
		/// <param name="port_no">�|�[�g�ԍ�</param>
		/// <remarks>
		/// POP3 �T�[�o�ɐڑ����܂��B
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	POP �T�[�o�Ƃ̐ڑ��Ɏ��s���܂����B
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Connect(int port_no)
		{
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// POP3 �T�[�o�Ƃ̐ڑ����I�����܂��B
		/// </summary>
		public void Close()
		{
			if(_socket != (IntPtr)ErrorSocket) {
				Pop3Close(_socket);
			}
			_socket = (IntPtr)ErrorSocket;
		}
		/// <summary>
		/// POP3 �T�[�o�F�؂��s���܂��B
		/// </summary>
		/// <remarks>
		/// POP3 �T�[�o�F�؂��s���܂��B
		/// </remarks>
		/// <param name="id_str">POP3 ���[�U�[ ID</param>
		/// <param name="pass_str">POP3 �p�X���[�h</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="FormatException">
		///	ID �������̓p�X���[�h�ɕ����񂪓����Ă��܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// POP3 �T�[�o�F�؂��s���܂��B
		/// </summary>
		/// <remarks>
		/// POP3 �T�[�o�F�؂��s���܂��B
		/// </remarks>
		/// <param name="id_str">POP3 ���[�U�[ ID</param>
		/// <param name="pass_str">POP3 �p�X���[�h</param>
		/// <param name="apop_flag">APOP ���g�p���邩</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="FormatException">
		///	ID �������̓p�X���[�h�ɕ����񂪓����Ă��܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Authenticate(string id_str, string pass_str, bool apop_flag)
		{
			_apop = apop_flag;
			Authenticate(id_str, pass_str);
		}

		/// <summary>
		/// ���[���̃X�e�[�^�X���擾���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�</param>
		/// <remarks>
		/// <paramref name="no"/>�p�����[�^�ŃX�e�[�^�X���擾���������[���ԍ����w�肵�܂��B
		/// <para>������<see cref="Subject"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���t�������<see cref="DateString"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���o�l��<see cref="From"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�w�b�_��<see cref="Header"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�X�e�[�^�X�擾���s�̏ꍇ�̃G���[�ԍ���<see cref="nMailException.ErrorCode"/>�Ŏ擾�ł��܂��B</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	���[���ԍ�������������܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���̃T�C�Y���擾���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�</param>
		/// <remarks>
		/// <paramref name="no"/>�p�����[�^�Ń��[���T�C�Y���擾���������[���ԍ����w�肵�܂��B
		/// <example>
		/// <para>
		/// ���[���ԍ�(�ϐ���:no)�̃��[���T�C�Y���擾����B
		/// <code lang="cs">
		///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
		///		try {
		///			pop.Connect();
		/// 		pop.Authenticate("pop3_id", "password");
		///			pop.GetSize(no);
		///			MessageBox.Show(String.Format("���[���ԍ�:{0:d},�T�C�Y:{1:d}", no, pop.Size));
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
		/// Try
		/// 	pop.Connect()
		/// 	pop.Authenticate("pop3_id", "password")
		///		pop.GetMail(no)
		///		MessageBox.Show(String.Format("���[���ԍ�:{0:d},�T�C�Y:{1:d}", no, pop.Size))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		pop.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	���[���ԍ�������������܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		/// <returns>���[���T�C�Y</returns>
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
		/// ���[�����擾���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�</param>
		/// <remarks>
		/// <paramref name="no"/>�p�����[�^�Ń��[�����擾���������[���ԍ����w�肵�܂��B
		/// <para>�Y�t�t�@�C����ۑ��������ꍇ�A<see cref="Path"/>�ɕۑ��������t�H���_���w�肵�Ă����܂��B</para>
		/// <para>�g���@�\���g�p�������ꍇ�A<see cref="Flag"/>�Őݒ肵�Ă����܂��B</para>
		/// <para>������<see cref="Subject"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���t�������<see cref="DateString"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���o�l��<see cref="From"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�w�b�_��<see cref="Header"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���[���T�C�Y��<see cref="Size"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�Y�t�t�@�C������<see cref="FileName"/>�Ŏ擾�ł��܂��B</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="DirectoryNotFoundException">
		///	<see cref="Path"/>�Ŏw�肵���t�H���_�����݂��܂���B
		/// </exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	���[���ԍ�������������܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[�����폜���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�</param>
		/// <remarks>
		/// <para>���[���폜���s�̏ꍇ�̃G���[�ԍ���<see cref="nMailException.ErrorCode"/>�Ŏ擾�ł��܂��B</para>
		/// <example>
		/// <para>
		/// �w��̃��[���ԍ�(�ϐ���:no)���폜����B
		/// <code lang="cs">
		///	using(nMail.Pop3 pop = new nMail.Pop3("mail.example.com")) {
		///		try {
		///			pop.Connect();
		/// 		pop.Authenticate("pop3_id", "password");
		///			pop.Delete(no);
		///			MessageBox.Show(String.Format("���[���ԍ�:{0:d}���폜����", no));
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
		/// Try
		/// 	pop.Connect()
		/// 	pop.Authenticate("pop3_id", "password")
		///		pop.Delete(no)
		///		MessageBox.Show(String.Format("���[���ԍ�:{0:d}���폜����", no))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		pop.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	���[���ԍ�������������܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// �Y�t�t�@�C���̑��݃`�F�b�N���s���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�</param>
		/// <remarks>
		/// <para>�Y�t�t�@�C���̕�������<see cref="PartNo"/>�Ŏ擾�ł��܂��B</para>
		/// <para><see cref="NoAttachmentFile"/>�̏ꍇ�Y�t�t�@�C���͂���܂���B</para>
		/// <para><see cref="AttachmentFile"/>�̏ꍇ��������Ă��Ȃ��Y�t�t�@�C���t�����[���ł��B</para>
		/// <para>1 �ȏ�̏ꍇ��������Ă���Y�t�t�@�C���ŕԂ�l�̓p�[�g�ԍ���\���܂��B</para>
		/// <para>1 �̏ꍇ�́A<see cref="Flag"/>��<see cref="PartialAttachmentFile"/> ��ݒ肵��
		/// <see cref="GetMail"/>���Ăяo���ƓY�t�t�@�C�����������ĕۑ��\�ł��B</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	���[���ԍ�������������܂���B
		/// </exception>
		/// <returns>true �œY�t�t�@�C��������</returns>
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
		/// ���[���� UIDL ���擾���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�</param>
		/// <remarks>
		/// <para>�擾���� UIDL ��<see cref="Uidl"/>�Ŏ擾�ł��܂��B</para>
		/// <para><paramref name="no"/>��<see cref="UidlAll"/>���w�肷��ƁA
		/// POP3 �T�[�o��ɂ��邷�ׂẴ��[���� UIDL ���擾���܂��B</para>
		/// <example>
		/// <para>
		/// �w��̃��[���ԍ�(�ϐ���:no)�� UIDL ���擾����B
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
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
		/// Try
		/// 	pop.Connect()
		/// 	pop.Authenticate("pop3_id", "password")
		///		pop.GetUidl(no)
		///		MessageBox.Show("Uidl=" + pop.Uidl)
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		pop.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���w�b�_����w��̃t�B�[���h�̓��e���擾���܂��B
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <returns>�t�B�[���h�̓��e</returns>
		/// <remarks>
		/// POP3 �T�[�o�Ƃ̐ڑ��Ƃ͖��֌W�Ɏg�p�ł��܂��B
		/// <para>�w�b�_�́A<see cref="Header"/>�Őݒ肵�Ă����܂��B
		/// <see cref="GetMail"/>�Ŏ�M��������ɌĂяo�����ꍇ�A
		/// ��M�������[���̃w�b�_���g�p���܂��B</para>
		/// <para>�擾�����t�B�[���h���e��<see cref="Field"/>�Ŏ擾�ł��܂��B</para>
		/// <example>
		/// <para>
		/// �w��̃��[���ԍ�(�ϐ�:no)�� X-Mailer �w�b�_�t�B�[���h���擾����B
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
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception e)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim pop As nMail.Pop3 = New nMail.Pop3("mail.example.com")
		///	Try
		/// 	pop.Connect()
		/// 	pop.Authenticate("pop3_id", "password")
		///		pop.GetMail(no)
		///		MessageBox.Show("X-Mailer:" + pop.GetHeaderField("X-Mailer:"))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		pop.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		/// <returns>�t�B�[���h�̓��e</returns>
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
		/// ���[���w�b�_����w��̃t�B�[���h�̓��e���擾���܂��B
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <param name="header">�w�b�_</param>
		/// <returns>�t�B�[���h�̓��e</returns>
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
		/// MIME �w�b�_�t�B�[���h�̕�������f�R�[�h���܂�
		/// </summary>
		/// <param name="field">�t�B�[���h�̕�����</param>
		/// <returns>�f�R�[�h�����t�B�[���h���e</returns>
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
		/// ���[���w�b�_����w��̃w�b�_�t�B�[���h�̓��e���擾���A
		/// MIME �w�b�_�t�B�[���h�̃f�R�[�h���s���ĕԂ��܂�
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <returns>�擾�����f�R�[�h�ς݂̃t�B�[���h���e</returns>
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
		/// ���[���w�b�_����w��̃w�b�_�t�B�[���h�̓��e���擾���A
		/// MIME �w�b�_�t�B�[���h�̃f�R�[�h���s���ĕԂ��܂�
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <param name="header">�w�b�_</param>
		/// <returns>�擾�����f�R�[�h�ς݂̃t�B�[���h���e</returns>
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
		/// �Y�t�t�@�C�����̔z��ł��B
		/// </summary>
		/// <returns>�Y�t�t�@�C�����̔z��</returns>
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
		/// POP3 �|�[�g�ԍ��ł��B
		/// </summary>
		/// <value>POP3 �|�[�g�ԍ�</value>
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
		/// �\�P�b�g�n���h���ł��B
		/// </summary>
		/// <value>�\�P�b�g�n���h��</value>
		public IntPtr Handle {
			get
			{
				return _socket;
			}
		}
		/// <summary>
		/// POP3 �T�[�o��̃��[�����ł��B
		/// </summary>
		/// <value>POP3 �T�[�o��̃��[����</value>
		public int Count {
			get
			{
				return _count;
			}
		}
		/// <summary>
		/// ���[���̃T�C�Y�ł��B
		/// </summary>
		/// <value>���[���̃T�C�Y</value>
		public int Size {
			get
			{
				return _size;
			}
		}
		/// <summary>
		/// POP3 �T�[�o���ł��B
		/// </summary>
		/// <value>POP3 �T�[�o��</value>
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
		/// POP3 ���[�U�[���ł��B
		/// </summary>
		/// <value>POP3 ���[�U�[��</value>
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
		/// POP3 �p�X���[�h�ł��B
		/// </summary>
		/// <value>POP3 �p�X���[�h</value>
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
		/// �Y�t�t�@�C����ۑ�����t�H���_�ł��B
		/// </summary>
		/// <remarks>
		/// null (VB.Net �� nothing) �̏ꍇ�ۑ����܂���B
		/// </remarks>
		/// <value>�Y�t�t�@�C���ۑ��t�H���_</value>
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
		/// ���[���̖{���ł��B
		/// </summary>
		/// <value>���[���{��</value>
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
		/// ���[���̌����ł��B
		/// </summary>
		/// <value>���[���̌���</value>
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
		/// ���[���̑��M�����̕�����ł��B
		/// </summary>
		/// <value>���[�����M����������</value>
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
		/// ���[���̍��o�l�ł��B
		/// </summary>
		/// <value>���[���̍��o�l</value>
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
		/// ���[���̃w�b�_�ł��B
		/// </summary>
		/// <value>���[���̃w�b�_</value>
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
		/// �Y�t�t�@�C�����ł��B
		/// </summary>
		/// <remarks>
		/// �����̓Y�t�t�@�C��������ꍇ�A"," �ŋ�؂��Ċi�[����܂��B
		/// <see cref="Options.SplitChar"/>�ŋ�؂蕶����ύX�ł��܂��B
		/// </remarks>
		/// <value>�Y�t�t�@�C����</value>
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
		/// �Y�t�t�@�C�����̔z��ł��B
		/// </summary>
		/// <remarks>
		/// ���̃v���p�e�B�͌݊����̂��߂Ɏc���Ă���܂��B
		///	<see cref="GetFileNameList"/>�Ŕz����擾���Ďg�p����悤�ɂ��Ă��������B
		/// </remarks>
		/// <value>�Y�t�t�@�C����</value>
		public string[] FileNameList
		{
			get
			{
				return _filename_list;
			}
		}
		/// <summary>
		/// �w�b�_�̃t�B�[���h���ł��B
		/// </summary>
		/// <value>�w�b�_�̃t�B�[���h��</value>
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
		/// �w�b�_�t�B�[���h�̓��e�ł��B
		/// </summary>
		/// <value>�w�b�_�t�B�[���h�̓��e</value>
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
		/// �Y�t�t�@�C���̕������ł��B
		/// </summary>
		/// <value>�Y�t�t�@�C���̕�����</value>
		public int PartNo
		{
			get
			{
				return _part_no;
			}
		}
		/// <summary>
		/// �Y�t�t�@�C���� ID �ł��B
		/// </summary>
		/// <value>�Y�t�t�@�C���� ID</value>
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
		/// ���[���� UIDL �ł��B
		/// </summary>
		/// <value>���[�� UIDL</value>
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
		/// APOP ���g�p���邩�ǂ����̃t���O�ł��B
		/// </summary>
		/// <remarks>
		/// true �� APOP ���g�p���܂��B
		/// </remarks>
		/// <value>APOP ���g�p���邩�ǂ����̃t���O</value>
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
		/// ���[���g���@�\�w��t���O�ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="PartialAttachmentFile"/>�������ꂽ�Y�t�t�@�C�����擾���܂��B</para>
		/// <para><see cref="TextOnly"/>���[���{���̂ݎ擾���܂��B</para>
		/// <para><see cref="SuspendAttachmentFile"/>�Y�t�t�@�C����M�ňꎞ�x�~����̈��ڂɎw�肵�܂��B</para>
		/// <para><see cref="SuspendNext"/>�Y�t�t�@�C����M�ňꎞ�x�~����̓��ڈȍ~�Ɏw�肵�܂��B</para>
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
		/// �G���[�ԍ��ł��B
		/// </summary>
		/// <value>�G���[�ԍ�</value>
		public int ErrorCode
		{
			get
			{
				return _err;
			}
		}
		/// <summary>
		/// text/html �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		/// <value>�t�@�C����</value>
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
		/// SSL �ݒ�t���O�ł��B
		/// </summary>
		/// <value>SSL �ݒ�t���O</value>
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
		/// SSL �N���C�A���g�ؖ������ł��B
		/// </summary>
		/// <remarks>
		/// null �̏ꍇ�w�肵�܂���B
		/// </remarks>
		/// <value>SSL �N���C�A���g�ؖ�����</value>
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
		/// message/rfc822 �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		/// <value>�t�@�C����</value>
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
	/// SMTP ���[�����M�N���X
	/// </summary>
	/// <example>
	/// <para>
	/// ���[���𑗐M����B
	///	<code lang="cs">
	///	using(nMail.Smtp smtp = new nMail.Smtp("mail.example.com")) {
	///		try {
	///			smtp.SendMail("to@example.net", "from@example.com", "�e�X�g", "�{����s��\r\n��s��");
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim smtp As nMail.Smtp = New nMail.Smtp("mail.example.com")
	///	Try
	/// 	smtp.SendMail("to@example.net", "from@example.com", "�e�X�g", "�{����s��" + ControlChars.CrLf + "��s��")
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		smtp.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// SMTP AUTH ���g�p���ă��[���𑗐M����B
	///	<code lang="cs">
	///	using(nMail.Smtp smtp = new nMail.Smtp("mail.example.com")) {
	///		try {
	///			smtp.Connect();
	///			smpt.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain | nMail.Smtp.AuthCramMd5);
	///			smtp.SendMail("to@example.net", "from@example.com", "�e�X�g", "�{����s��\r\n��s��");
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim smtp As nMail.Smtp = New nMail.Smtp("mail.example.com")
	///	Try
	/// 	smtp.Connect()
	/// 	smtp.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain Or nMail.Smtp.AuthCramMd5)
	/// 	smtp.SendMail("to@example.net", "from@example.com", "�e�X�g", "�{����s��" + ControlChars.CrLf + "��s��")
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		smtp.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// TLS Version 1 �� SMTP AUTH ���g�p���ă��[���𑗐M����B
	///	<code lang="cs">
	///	using(nMail.Smtp smtp = new nMail.Smtp("mail.example.com")) {
	///		try {
	///			smtp.SSL = nMail.Smtp.TLS1;
	///			smtp.Connect(nMail.Smtp.StandardSslPortNo);		// over SSL/TLS �̃|�[�g�ԍ����w�肵�Đڑ�
	///			smpt.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain | nMail.Smtp.AuthCramMd5);
	///			smtp.SendMail("to@example.net", "from@example.com", "�e�X�g", "�{����s��\r\n��s��");
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim smtp As nMail.Smtp = New nMail.Smtp("mail.example.com")
	///	Try
	/// 	smtp.SSL = nMail.Smtp.TLS1
	/// 	smtp.Connect(nMail.Smtp.StandardSslPortNo)		' over SSL/TLS �̃|�[�g�ԍ����w�肵�Đڑ�
	/// 	smtp.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain Or nMail.Smtp.AuthCramMd5)
	/// 	smtp.SendMail("to@example.net", "from@example.com", "�e�X�g", "�{����s��" + ControlChars.CrLf + "��s��")
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		smtp.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// �ꎞ�x�~�@�\���g���ă��[���𑗐M����B
	///	<code lang="cs">
	///	using(nMail.Smtp smtp = new nMail.Smtp("mail.example.com")) {
	///		try {
	///			// �Y�t�t�@�C���̎w��
	///			smtp.FileName = @"z:\file.dat";
	///			smtp.Flag = nMail.Smtp.SuspendAttachmentFile;
	///			smtp.Connect();
	///			// �ꎞ�x�~�@�\�̂ݎg���ꍇ�ł� Authenticate �̌Ăяo���͕K�v�ł��B
	///			// Authenticate("", "", AuthNotUse); ���ł��B
	///			smtp.Authenticate();
	///			smtp.SendMail("to@example.net", "from@example.com", "�e�X�g", "�{����s��\r\n��s��");
	///			smtp.Flag = nMail.Smtp.SuspendNext;
	///			while(smtp.ErrorCode == nMail.Smtp.ErrorSuspendAttachmentFile)
	///			{
	///				smtp.SendMail();
	///				// �x�~���̏���������
	///				Application.DoEvents();
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim smtp As nMail.Smtp = New nMail.Smtp("mail.example.com")
	///	Try
	///		smtp.FileName = "z:\file.dat"
	/// 	smtp.Flag = nMail.Smtp.SuspendAttachmentFile
	///		smtp.Connect()
	/// 	smtp.Authenticate();
	/// 	smtp.SendMail("to@example.net", "from@example.com", "�e�X�g", "�{����s��" + ControlChars.CrLf + "��s��")
	///		smtp.Flag = nMail.Smtp.SuspendNext
	///		Do While smtp.ErrorCode = nMail.Smtp.ErrorSuspendAttachmentFile
	///			smtp.SendMail()
	///			' �x�~���̏���������
	///			Application.DoEvents()
	///		Loop
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		smtp.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// SMTP AUTH ���g�p���A����Ɉꎞ�x�~�@�\���g���ă��[���𑗐M����B
	///	<code lang="cs">
	///	using(nMail.Smtp smtp = new nMail.Smtp("mail.example.com")) {
	///		try {
	///			// �Y�t�t�@�C���̎w��
	///			smtp.FileName = @"z:\file.dat";
	///			smtp.Flag = nMail.Smtp.SuspendAttachmentFile;
	///			smtp.Connect();
	///			smpt.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain | nMail.Smtp.AuthCramMd5);
	///			smtp.SendMail("to@example.net", "from@example.com", "�e�X�g", "�{����s��\r\n��s��");
	///			smtp.Flag = nMail.Smtp.SuspendNext;
	///			while(smtp.ErrorCode == nMail.Smtp.ErrorSuspendAttachmentFile)
	///			{
	///				smtp.SendMail();
	///				// �x�~���̏���������
	///				Application.DoEvents();
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim smtp As nMail.Smtp = New nMail.Smtp("mail.example.com")
	///	Try
	///		smtp.FileName = "z:\file.dat"
	/// 	smtp.Flag = nMail.Smtp.SuspendAttachmentFile
	///		smtp.Connect()
	///		smpt.Authenticate("smtp_id", "password", nMail.Smtp.AuthPlain Or nMail.Smtp.AuthCramMd5)
	/// 	smtp.SendMail("to@example.net", "from@example.com", "�e�X�g", "�{����s��" + ControlChars.CrLf + "��s��")
	///		smtp.Flag = nMail.Smtp.SuspendNext
	///		Do While smtp.ErrorCode = nMail.Smtp.ErrorSuspendAttachmentFile
	///			smtp.SendMail()
	///			' �x�~���̏���������
	///			Application.DoEvents()
	///		Loop
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		smtp.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// </example>
	public class Smtp : IDisposable
	{
		/// <summary>
		/// �g���@�\�p�o�b�t�@�̃T�C�Y�ł��B
		/// </summary>
		protected const int AttachmentTempSize = 400;

		/// <summary>
		/// �\�P�b�g�G���[�������͔�ڑ���Ԃł��B�l�� -1 �ł��B
		/// </summary>
		public const int ErrorSocket = -1;
		/// <summary>
		/// �F�؎��s�G���[�ł��B�l�� -2 �ł��B
		/// </summary>
		public const int ErrorAuthenticate = -2;
		/// <summary>
		/// �^�C���A�E�g�G���[�ł��B�l�� -4 �ł��B
		/// </summary>
		public const int ErrorTimeout = -4;
		/// <summary>
		/// �Y�t�t�@�C���I�[�v�����s�G���[�ł��B�l�� -5 �ł��B
		/// </summary>
		public const int ErrorFileOpen = -5;
		/// <summary>
		/// �T�[�o���w�肵���F�،`���ɑΉ����Ă��܂���B�l�� -8 �ł��B
		/// </summary>
		public const int ErrorAuthenticateNoSupport = -8;
		/// <summary>
		/// �������m�ۃG���[�ł��B�l�� -9 �ł��B
		/// </summary>
		public const int ErrorMemory = -9;
		/// <summary>
		/// ���̑��̃G���[�ł��B�l�� -10 �ł��B
		/// </summary>
		public const int ErrorEtc = -10;
		/// <summary>
		/// ���[�����M���̈ꎞ�x�~��Ԃł��B�l�� -20 �ł��B
		/// </summary>
		public const int ErrorSuspendAttachmentFile = -20;
		/// <summary>
		/// �T�[�r�X�����p�ł��܂���B�l�� -421 �ł��B
		/// </summary>
		public const int ErrorServiceNotAvaliable = -421;
		/// <summary>
		/// ���[���{�b�N�X���g�p�ł��܂���B�i�g�p�����j�l�� -450 �ł��B
		/// </summary>
		public const int ErrorMailboxUnavalable = -450;
		/// <summary>
		/// �T�[�o�����G���[�ł��B�l�� -451 �ł��B
		/// </summary>
		public const int ErrorLocal = -451;
		/// <summary>
		/// �V�X�e���e�ʕs���G���[�ł��B�l�� -452 �ł��B
		/// </summary>
		public const int ErrorInsufficientSystemStorage = -452;
		/// <summary>
		/// �R�}���h�̕��@�G���[�ł��B�l�� -500 �ł��B
		/// </summary>
		public const int ErrorSyntax = -500;
		/// <summary>
		/// �p�����[�^�������̕��@�G���[�ł��B�l�� -501 �ł��B
		/// </summary>
		public const int ErrorParameter = -501;
		/// <summary>
		/// �������R�}���h�ł��B�l�� -502 �ł��B
		/// </summary>
		public const int ErrorCommandNotImplemented = -502;
		/// <summary>
		/// �R�}���h�̃V�[�P���X�ُ�ł��B�l�� -503 �ł��B
		/// </summary>
		public const int ErrorBadSequence =-503;
		/// <summary>
		/// SMTP �F�؎��s�ł��B�l�� -535 �ł��B
		/// </summary>
		public const int ErrorSmtpAuthenticate = -535;
		/// <summary>
		/// ���M�悪����܂���B�l�� -550 �ł��B
		/// </summary>
		public const int ErrorUserUnkown = -550;
		/// <summary>
		/// ���[�U�[�����݂��Ȃ����A�]���悪����܂���B�l�� -551 �ł��B
		/// </summary>
		public const int ErrorUserNotLocal = -551;
		/// <summary>
		/// �e�ʃI�[�o�[�G���[�ł��B�l�� -552 �ł��B
		/// </summary>
		public const int ErrorExceededStorageAllocation =-552;
		/// <summary>
		/// ���[���{�b�N�X��������������܂���B�l�� -553 �ł��B
		/// </summary>
		public const int ErrorMailboxNameNotAllowed = -553;
		/// <summary>
		/// �����[�h�~�G���[�ł��B�l�� -553 �ł��B
		/// </summary>
		public const int ErrorRelayOperationRejected =-553;
		/// <summary>
		/// �g�����U�N�V�����Ɏ��s���܂����B�l�� -554 �ł��B
		/// </summary>
		public const int ErrorTransactionFailed = -554;
		/// <summary>
		/// ���M�����t�B�[���h�� nMail.DLL ���Ő������܂��B
		/// </summary>
		public const int AddDateField = 1;
		/// <summary>
		/// Message-ID �t�B�[���h�� nMail.DLL ���Ő������܂��B
		/// </summary>
		public const int AddMessageId = 2;
		/// <summary>
		/// �傫�ȃT�C�Y�̓Y�t�t�@�C�����M���Ɉ�U������߂��܂��B
		/// </summary>
		public const int SuspendAttachmentFile = 4;
		/// <summary>
		/// �傫�ȃT�C�Y�̓Y�t�t�@�C�����M���ڈȍ~�ł��B
		/// </summary>
		public const int SuspendNext = 8;
		/// <summary>
		/// RFC2231 �œY�t�t�@�C�������G���R�[�h���܂��B
		/// </summary>
		public const int FileNameRfc2231 = 16;
		/// <summary>
		/// UTF-8 �Ń��[�����G���R�[�h���܂��i�����BASE64�ŃG���R�[�h����܂��j
		/// </summary>
		public const int EncodeUtf8 = 32;
		/// <summary>
		/// HTML ���[���𑗐M���܂��B
		/// </summary>
		public const int HtmlMail = 64;
		/// <summary>
		/// text/html �p�[�g���t�@�C���Ŏw�肵�܂��B
		/// </summary>
		public const int HtmlUseFile = 128;
		/// <summary>
		/// HTML ���[���̈���̃h���C�����݂� multipart �̍\�������肵�܂��B
		/// </summary>
		public const int HtmlAutoPart = 256;
		/// <summary>
		/// text/html �p�[�g�Ŏg�p����摜�t�@�C����Y�t����ꍇ�Amultipart/mixed �Ȃ��ő��M���܂��B
		/// </summary>
		public const int HtmlNoMixedPart = 512;
		/// <summary>
		/// Softbank �g�ь����I�v�V�����iHtmlNoMixedPart�Ɠ����j
		/// </summary>
		public const int HtmlSoftbank = 512;
		/// <summary>
		/// EMOBILE �g�ь����I�v�V�����iHtmlNoMixedPart�Ɠ����j
		/// </summary>
		public const int HtmlEmobile = 512;
		/// <summary>
		/// WILLCOM �g�ь����I�v�V�����iHtmlNoMixedPart�Ɠ����j
		/// </summary>
		public const int HtmlWillcom = 512;
		/// <summary>
		/// text/html �p�[�g�Ŏg�p����摜�t�@�C����Y�t����ꍇ�Amultipart/related �Ȃ��ő��M���܂��B
		/// </summary>
		public const int HtmlNoRelatedPart = 1024;
		/// <summary>
		/// au �g�ь����I�v�V�����iHtmlNoRelatedPart�Ɠ����j
		/// </summary>
		public const int HtmlAu = 1024;
		/// <summary>
		/// text/html �p�[�g�̂݁itext/plain �p�[�g�Ȃ��j�� HTML ���[���𑗐M���܂��B
		/// </summary>
		public const int HtmlNoPlainPart = 2048;
		/// <summary>
		/// text/plain �p�[�g�Ŏg�p���镶����� Body �Ŏw�肵�܂��B
		/// </summary>
		public const int HtmlSetPlainBody = 4096;
		/// <summary>
		/// ���������ł��B
		/// </summary>
		public const int Success = 1;
		/// <summary>
		/// SMTP AUTH �͎g�p���܂���B
		/// </summary>
		public const int AuthNotUse = 0;
		/// <summary>
		/// SMTP AUTH PLAIN ���g�p���܂��B
		/// </summary>
		public const int AuthPlain = 1;
		/// <summary>
		/// SMTP AUTH LOGIN ���g�p���܂��B
		/// </summary>
		public const int AuthLogin = 2;
		/// <summary>
		/// SMTP AUTH CRAM MD5 ���g�p���܂��B
		/// </summary>
		public const int AuthCramMd5 = 4;
		/// <summary>
		/// SMTP AUTH DIGEST MD5 ���g�p���܂��B
		/// </summary>
		public const int AuthDigestMd5 = 8;
		/// <summary>
		/// SSLv3 ���g�p���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int SSL3 = 0x1000;
		/// <summary>
		/// TSLv1 ���g�p���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int TLS1 = 0x2000;
		/// <summary>
		/// STARTTLS ���g�p���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int STARTTLS = 0x4000;
		/// <summary>
		/// �T�[�o�ؖ����������؂�ł��G���[�ɂ��܂���B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int IgnoreNotTimeValid = 0x0800;
		/// <summary>
		/// ���[�g�ؖ����������ł��G���[�ɂ��܂���B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int AllowUnknownCA = 0x0400;
		/// <summary>
		/// SMTP �̕W���|�[�g�ԍ��ł�
		/// </summary>
		public const int StandardPortNo = 25;
		/// <summary>
		/// SMTP �̃T�u�~�b�V�����|�[�g�ԍ��ł�
		/// </summary>
		public const int SubmissionPortNo = 587;
		/// <summary>
		/// SMTP over SSL �̃|�[�g�ԍ��ł�
		/// </summary>
		public const int StandardSslPortNo = 465;

		/// <summary>
		/// SMTP �|�[�g�ԍ��ł��B
		/// </summary>
		protected int _port = 25;
		/// <summary>
		/// �\�P�b�g�n���h���ł��B
		/// </summary>
		protected IntPtr _socket = (IntPtr)ErrorSocket;
		/// <summary>
		/// ���[�����M�g���@�\�̎w��ł��B
		/// </summary>
		protected int _flag = 0;
		/// <summary>
		/// �G���[�ԍ��ł��B
		/// </summary>
		protected int _err = 0;
		/// <summary>
		/// SMTP AUTH �̔F�ؕ��@�ł��B
		/// </summary>
		protected int _mode = AuthNotUse;
		/// <summary>
		/// SMTP �T�[�o���ł��B
		/// </summary>
		protected string _host = "";
		/// <summary>
		/// SMTP AUTH ���[�U�[���ł��B
		/// </summary>
		protected string _id = "";
		/// <summary>
		/// SMTP AUTH �p�X���[�h�ł��B
		/// </summary>
		protected string _password = "";
		/// <summary>
		/// ���[���̈���ł��B
		/// </summary>
		protected string _to = "";
		/// <summary>
		/// ���[���� CC �ł��B
		/// </summary>
		protected string _cc = "";
		/// <summary>
		/// ���[���� BCC �ł��B
		/// </summary>
		protected string _bcc = "";
		/// <summary>
		/// ���[���̍��o�l�ł��B
		/// </summary>
		protected string _from = "";
		/// <summary>
		/// ���[���̌����ł��B
		/// </summary>
		protected string _subject = "";
		/// <summary>
		/// ���[���̖{���ł��B
		/// </summary>
		protected string _body = "";
		/// <summary>
		/// ���[���̒ǉ��w�b�_�ł��B
		/// </summary>
		protected string _header = "";
		/// <summary>
		/// ���[���ɓY�t����t�@�C�����ł��B
		/// </summary>
		protected string _filename = "";
		/// <summary>
		/// �Y�t�t�@�C�����̃��X�g�ł��B
		/// </summary>
		protected string[] _filename_list = null;
		/// <summary>
		/// �g���@�\�p�o�b�t�@�ł��B
		/// </summary>
		protected byte[] _temp = null;
		/// <summary>
		/// Connect() ���g�������ǂ����̃t���O�ł��B
		/// </summary>
		protected bool _connect_flag = false;
		/// <summary>
		/// Dispose �������s�������ǂ����̃t���O�ł��B
		/// </summary>
		private bool _disposed = false;
		/// <summary>
		/// SSL �ݒ�t���O�ł��B
		/// </summary>
		protected int _ssl = 0;
		/// <summary>
		/// SSL �N���C�A���g�ؖ������ł��B
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
		/// <c>Smtp</c>�N���X�̐V�K�C���X�^���X�����������܂��B
		/// </summary>
		public Smtp()
		{
			Init();
		}
		/// <summary>
		/// <c>Smtp</c>�N���X�̐V�K�C���X�^���X�����������܂��B
		/// </summary>
		/// <param name="host_name">SMTP �T�[�o��</param>
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
		/// <see cref="Smtp"/>�ɂ���Ďg�p����Ă��邷�ׂẴ��\�[�X��������܂��B
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// �����������ł��B
		/// </summary>
		protected void Init()
		{
			_temp = new byte[AttachmentTempSize];
		}
		/// <summary>
		/// <see cref="Smtp"/>�ɂ���Ďg�p����Ă��邷�ׂẴ��\�[�X��������܂��B
		/// </summary>
		/// <param name="disposing">
		/// �}�l�[�W���\�[�X�ƃA���}�l�[�W���\�[�X�̗������������ꍇ��<c>true</c>�B
		/// �A���}�l�[�W���\�[�X�������������ꍇ��<c>false</c>�B
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
		/// ���[���𑗐M���܂��B
		/// </summary>
		/// <remarks>
		///	�ꎞ�x�~�@�\�̓��ڈȍ~�̌Ăяo���Ŏg�p���܂��B
		/// </remarks>
		/// <exception cref="FileNotFoundException">
		/// <see cref="FileName"/>�Őݒ肳��Ă���t�@�C�������݂��܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���𑗐M���܂��B
		/// </summary>
		/// <param name="to_str">����</param>
		/// <param name="from_str">���o�l</param>
		/// <param name="subject_str">����</param>
		/// <param name="body_str">�{��</param>
		/// <remarks>
		/// <paramref name="to_str"/>�p�����[�^�Ń��[���̈�����w�肵�܂��B
		/// <paramref name="from_str"/>�p�����[�^�ō��o�l���w�肵�܂��B
		/// <paramref name="subject_str"/>�p�����[�^�Ō������w�肵�܂��B
		/// <paramref name="body_str"/>�p�����[�^�Ŗ{�����w�肵�܂��B
		/// <para>CC ���w�肵�����ꍇ�A<see cref="Cc"/>�Ɏw�肵�Ă����܂��B</para>
		/// <para>BCC ���w�肵�����ꍇ�A<see cref="Bcc"/>�Ɏw�肵�Ă����܂��B</para>
		/// <para>�t�@�C����Y�t�������ꍇ�A<see cref="FileName"/>�Ɏw�肵�Ă����܂��B</para>
		/// </remarks>
		/// <exception cref="FileNotFoundException">
		/// <see cref="FileName"/>�Őݒ肳��Ă���t�@�C�������݂��܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// SMTP �T�[�o�ɐڑ����܂��B
		/// </summary>
		/// <remarks>
		///	SMTP AUTH ����шꎞ�x�~�@�\���g���ꍇ�Ɏg�p���܂��B
		/// </remarks>
		///	<exception cref="FormatException">
		///	�T�[�o���ɕ����񂪓����Ă��܂���B
		/// </exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// SMTP �T�[�o�ɐڑ�
		/// </summary>
		/// <param name="host_name">SMTP �T�[�o��</param>
		/// <remarks>
		///	SMTP AUTH ����шꎞ�x�~�@�\���g���ꍇ�Ɏg�p���܂��B
		/// </remarks>
		///	<exception cref="FormatException">
		///	�T�[�o���ɕ����񂪓����Ă��܂���B
		/// </exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Connect(string host_name)
		{
			_host = host_name;
			Connect();
		}
		/// <summary>
		/// SMTP �T�[�o�ɐڑ�
		/// </summary>
		/// <param name="host_name">SMTP �T�[�o��</param>
		/// <param name="port_no">�|�[�g�ԍ�</param>
		/// <remarks>
		///	SMTP AUTH ����шꎞ�x�~�@�\���g���ꍇ�Ɏg�p���܂��B
		/// </remarks>
		///	<exception cref="FormatException">
		///	�T�[�o���ɕ����񂪓����Ă��܂���B
		/// </exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Connect(string host_name, int port_no)
		{
			_host = host_name;
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// SMTP �T�[�o�ɐڑ�
		/// </summary>
		/// <param name="port_no">�|�[�g�ԍ�</param>
		/// <remarks>
		///	SMTP AUTH ����шꎞ�x�~�@�\���g���ꍇ�Ɏg�p���܂��B
		/// </remarks>
		///	<exception cref="FormatException">
		///	�T�[�o���ɕ����񂪓����Ă��܂���B
		/// </exception>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Connect(int port_no)
		{
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// SMTP �T�[�o�Ƃ̐ڑ��I��
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
		/// SMTP AUTH �F��
		/// </summary>
		///	<remarks>
		///	�ꎞ�x�~�@�\���g���ꍇ��<see cref="Connect"/>�̌�ɌĂяo���܂��B
		///	</remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo���Ă��܂���B���̃��\�b�h���Ăяo���ꍇ��(<see cref="Connect"/>���Ăяo���K�v������܂��B�B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// SMTP AUTH �F��
		/// </summary>
		/// <param name="id_str">���[�U�[ ID</param>
		/// <param name="pass_str">�p�X���[�h</param>
		/// <param name="mode">�F�،`��</param>
		/// <para>�F�،`���̐ݒ�\�Ȓl�͉��L�̒ʂ�ł��B</para>
		/// <para><see cref="AuthPlain"/> �� PLAIN ���g�p���܂��B</para>
		/// <para><see cref="AuthLogin"/> �� LOGIN ���g�p���܂��B</para>
		/// <para><see cref="AuthCramMd5"/> CRAM MD5 ���g�p���܂��B</para>
		/// <para>AuthNotUse �ȊO�� C# �� | �AVB.NET �ł� Or �ŕ����w��\�ł��BCRAM MD5��LOGIN��PLAIN �̗D�揇�ʂŔF�؂����݂܂��B</para>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo���Ă��܂���B���̃��\�b�h���Ăяo���ꍇ��(<see cref="Connect"/>���Ăяo���K�v������܂��B�B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Authenticate(string id_str, string pass_str, int mode)
		{
			_id = id_str;
			_password = pass_str;
			_mode = mode;
			Authenticate();
		}
		/// <summary>
		/// SMTP �|�[�g�ԍ��ł��B
		/// </summary>
		/// <value>SMTP �|�[�g�ԍ�</value>
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
		/// �\�P�b�g�n���h���ł��B
		/// </summary>
		/// <value>SMTP �\�P�b�g�n���h��</value>
		public IntPtr Handle {
			get
			{
				return _socket;
			}
		}
		/// <summary>
		/// SMTP �T�[�o���ł��B
		/// </summary>
		/// <value>SMTP �T�[�o��</value>
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
		/// SMTP AUTH �p�̃��[�U�[���ł��B
		/// </summary>
		/// <value>SMTP AUTH ���[�U�[��</value>
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
		/// SMTP AUTH �p�̃p�X���[�h�ł��B
		/// </summary>
		/// <value>SMTP AUTH �p�X���[�h</value>
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
		/// ���[���̈���ł��B
		/// </summary>
		/// <remarks>
		/// �����̈���ɑ��M����ꍇ�A ',' �ŋ�؂��ă��[���A�h���X���L�q���Ă��������B
		/// </remarks>
		/// <value>���[�� ����</value>
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
		/// ���[���� CC �ł��B
		/// </summary>
		/// <remarks>
		/// �����̈���ɑ��M����ꍇ�A ',' �ŋ�؂��ă��[���A�h���X���L�q���Ă��������B
		/// </remarks>
		/// <value>���[�� CC</value>
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
		/// ���[���� BCC �ł��B
		/// </summary>
		/// <remarks>
		/// �����̈���ɑ��M����ꍇ�A ',' �ŋ�؂��ă��[���A�h���X���L�q���Ă��������B
		/// </remarks>
		/// <value>���[�� BCC</value>
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
		/// ���[���̍��o�l�ł��B
		/// </summary>
		/// <value>���[�����o�l</value>
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
		/// ���[���̌����ł��B
		/// </summary>
		/// <value>���[������</value>
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
		/// ���[���̖{���ł��B
		/// </summary>
		/// <value>���[���{��</value>
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
		/// ���[���̓Y�t�t�@�C���ł��B
		/// </summary>
		/// <remarks>
		/// �����̓Y�t�t�@�C���𑗐M����ꍇ�A"," �ŋ�؂�܂��B
		/// <see cref="Options.SplitChar"/>�ŋ�؂蕶����ύX�ł��܂��B
		/// </remarks>
		/// <value>�Y�t�t�@�C��</value>
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
		/// �Y�t�t�@�C�����̔z��ł��B
		/// </summary>
		/// <value>�Y�t�t�@�C�����̔z��</value>
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
		/// ���[���̒ǉ��w�b�_�ł��B
		/// </summary>
		/// <remarks>
		/// �����̃w�b�_��ǉ�����ꍇ�AC# �ł� '\r\n'�AVisual Basic �ł� ControlChars.CrLf
		/// �ŘA�����Ă��������B
		/// </remarks>
		/// <value>�ǉ��w�b�_</value>
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
		/// ���[�����M�̃I�v�V�����w��ł��B
		/// </summary>
		/// <value>���M�I�v�V����</value>
		/// <remarks>
		/// <para><see cref="AddDateField"/>���M�����t�B�[���h�� nMail.DLL ���Ő������܂��B</para>
		/// <para><see cref="AddMessageId"/>Message-ID �t�B�[���h�� nMail.DLL ���Ő������܂��B</para>
		/// <para><see cref="SuspendAttachmentFile"/>�Y�t�t�@�C�����M�ňꎞ�x�~����̈��ڂɎw�肵�܂��B</para>
		/// <para><see cref="SuspendNext"/>�Y�t�t�@�C�����M�ňꎞ�x�~����̓��ڈȍ~�Ɏw�肵�܂��B</para>
		///	<para><see cref="FileNameRfc2231"/>RFC2231 �œY�t�t�@�C�������G���R�[�h���܂��B</para>
		/// <para><see cref="EncodeUtf8"/>UTF-8 �Ń��[�����G���R�[�h���܂��i�����BASE64�ŃG���R�[�h����܂��j</para>
		/// <para>C# �̏ꍇ |�AVB.NET �̏ꍇ Or �ŕ����̎w�肪�\�ł��B������ SuspendAttachmentFile �� SuspendNext �͓����Ɏw��ł��܂���B</para>
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
		/// SMTP AUTH �̎�ʂł��B
		/// </summary>
		/// <remarks>
		/// <see cref="AuthNotUse"/> �� SMTP AUTH �͎g�p���܂���B
		/// <see cref="AuthPlain"/> �� PLAIN ���g�p���܂��B
		/// <see cref="AuthLogin"/> �� LOGIN ���g�p���܂��B
		/// <see cref="AuthCramMd5"/> CRAM MD5 ���g�p���܂��B
		/// <see cref="AuthDigestMd5"/> DIGEST MD5 ���g�p���܂��B
		/// AuthNotUse �ȊO�� C# �� | �AVisual Basic �ł� or �ŕ����ݒ�\�ł��B
		/// DIGEST MD5��CRAM MD5��LOGIN��PLAIN �̏��ԂŔF�؂����݂܂��B
		///	�f�t�H���g�l�� AuthNotUse �ł��B
		/// </remarks>
		/// <value>���M���[�h</value>
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
		/// ���[�����M���̃G���[�ԍ��ł��B
		/// </summary>
		/// <value>�G���[�ԍ�</value>
		public int ErrorCode
		{
			get
			{
				return _err;
			}
		}
		/// <summary>
		/// SSL �ݒ�t���O�ł��B
		/// </summary>
		/// <value>SSL �ݒ�t���O</value>
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
		/// SSL �N���C�A���g�ؖ������ł��B
		/// </summary>
		/// <remarks>
		/// null �̏ꍇ�w�肵�܂���B
		/// </remarks>
		/// <value>SSL �N���C�A���g�ؖ�����</value>
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
	/// �Y�t�t�@�C���ۑ��N���X
	/// </summary>
	/// <example>
	/// <para>
	///	�������ꂽ���[���f�[�^��ǂݏo���A�������ĕۑ�����B
	/// text/html �p�[�g���t�@�C���ɕۑ�����ꍇ�A<see cref="Options.DisableDecodeBodyText()"/> �������� <see cref="Options.DisableDecodeBodyAll()"/> ���Ă񂾌�� Pop3.GetMail() ���Ăяo����
	/// �擾�����w�b�_����і{���f�[�^���g�p���A<see cref="Attachment.Save()"/> �̑O�� <see cref="Options.EnableDecodeBody()"/> �� <see cref="Options.EnableSaveHtmlFile()"/> ���Ă�ł����K�v������܂��B
	/// <code lang="cs">
	///	using(nMail.Attachment attach = new nMail.Attachment()) {
	///		try {
	///			attach.Path = @"z:\temp";
	///			// count �͕�����
	///			for(int no = 0 ; no &lt; count ; no++) {
	///				// Read() �� header �Ƀw�b�_�{"\r\n"�{"�{��"��ǂݏo������
	///				string header = Read(no)
	///				attach.Add(header);
	///			}
	///			attach.Save();
	///			// ����I��
	///			MessageBox.Show(attach.Path + " �ɓY�t�t�@�C�� " + attach.FileName + " ��ۑ����܂����B\r\n\r\n����:" + attach.Subject + "\r\n�{��:\r\n" + attach.Body);
	///		}
	///		catch(nMailException nex) {
	///				if(nex.ErrorCode == nMail.Attachment.ErrorFileOpen) {
	///					MessageBox.Show("�Y�t�t�@�C�����I�[�v���ł��܂���B");
	///				} else if(nex.ErrorCode == nMail.Attachment.ErrorInvalidNo) {
	///	                MessageBox.Show("�������ꂽ���[���̏��Ԃ��������Ȃ����A�Y�����Ȃ��t�@�C���������Ă��܂��B")
	///				} else if(nex.ErrorCode == nMail.Attachment.ErrorPartial) {
	///					MessageBox.Show("�������ꂽ���[�����S�đ����Ă��܂���");
	///				}
	///			} catch(Exception ex) {
	///				MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///			}
	///		}
	///	}
	/// </code>
	/// </para>
	/// <para>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	///	Dim attach As nMail.Attachment = New nMail.Attachment
	///	Try
	///		Dim no As Integer
	///		attach.Path = "z:\temp"
	///		// count �͕�����
	///		For no = 0 To count - 1
	///			' Read() �� header �Ƀw�b�_�{ControlChars.CrLf�{"�{��"��ǂݏo������
	///			string header = Read(no)
	///			attach.Add(header)
	///		Next no
	///		attach.Save()
	///		' ����I��
	///		MessageBox.Show(attach.Path + " �ɓY�t�t�@�C�� " + attach.FileName + " ��ۑ����܂����B" + ControlChars.CrLf + ControlChars.CrLf + "����:" + attach.Subject + ControlChars.CrLf + "�{��:" + ControlChars.CrLf + attach.Body)
	///	Catch nex As nMail.nMailException
	///		If nex.ErrorCode = nMail.Attachment.ErrorFileOpen Then
	///			MessageBox.Show("�Y�t�t�@�C�����I�[�v���ł��܂���B")
	///		ElseIf nex.ErrorCode = nMail.Attachment.ErrorInvalidNo Then
	///			MessageBox.Show("�������ꂽ���[���̏��Ԃ��������Ȃ����A�Y�����Ȃ��t�@�C���������Ă��܂��B")
	///		ElseIf nex.ErrorCode = nMail.Attachment.ErrorPartial Then
	///			MessageBox.Show("�������ꂽ���[�����S�đ����Ă��܂���")
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
		/// �w�b�_�̈�̃T�C�Y�ł��B
		/// </summary>
		protected const int HeaderSize = 32768;
		/// <summary>
		/// �p�X������̃T�C�Y�ł��B
		/// </summary>
		protected const int MaxPath = 32768;
		/// <summary>
		/// �Y�t�t�@�C���W�J�p�o�b�t�@�̃T�C�Y�ł��B
		/// </summary>
		protected const int AttachmentTempSize = 400;

		/// <summary>
		/// �������ꂽ�Y�t�t�@�C���̏��Ԃ�����������܂���B�l�� -3 �ł��B
		/// </summary>
		public const int ErrorInvalidNo = -3;
		/// <summary>
		/// �Y�t�t�@�C�����쐬�ł��܂���ł����B�l�� -5 �ł��B
		/// </summary>
		public const int ErrorFileOpen = -5;
		/// <summary>
		/// �Y�t�t�@�C���̓W�J���I�����Ă��܂���B�l�� -6 �ł��B
		/// </summary>
		public const int ErrorPartial = -6;
		/// <summary>
		/// �Y�t�t�@�C���Ɠ����̃t�@�C�����t�H���_�ɑ��݂��܂��B�l�� -7 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="nMail.Options.AlreadyFile"/> �� <see cref="nMail.Options.FileAlreadyError"/></para>
		/// <para>��ݒ肵�A<see cref="Path"/>�Ŏw�肵���t�H���_�ɓY�t�t�@�C���Ɠ������O�̃t�@�C����</para>
		/// <para>����ꍇ�ɂ��̃G���[���������܂��B</para>
		/// </remarks>
		public const int ErrorFileAlready = -7;
		/// <summary>
		/// �������m�ۃG���[�ł��B�l�� -9 �ł��B
		/// </summary>
		public const int ErrorMemory = -9;
		/// <summary>
		/// ���̑��̃G���[�ł��B�l�� -10 �ł��B
		/// </summary>
		public const int ErrorEtc = -10;
		/// <summary>
		/// �Y�t�t�@�C���͑��݂��܂���B
		/// </summary>
		public const int NoAttachmentFile = -1;
		/// <summary>
		/// �w�b�_�f�[�^�̃��X�g
		/// </summary>
		protected ArrayList _header_list;
		/// <summary>
		/// �{���f�[�^�̃��X�g
		/// </summary>
		protected ArrayList _body_list;

		/// <summary>
		/// �w�b�_�[�T�C�Y�ł��B
		/// </summary>
		protected int _header_size = -1;
		/// <summary>
		/// �G���[�ԍ��ł��B
		/// </summary>
		protected int _err;
		/// <summary>
		/// �Y�t�t�@�C����ۑ�����p�X�ł��B
		/// </summary>
		protected string _path = "";
		/// <summary>
		/// ���[���̌����ł��B
		/// </summary>
		protected StringBuilder _subject;
		/// <summary>
		/// ���[���̑��M�����ł��B
		/// </summary>
		protected StringBuilder _date;
		/// <summary>
		/// ���[���̍��o�l�ł��B
		/// </summary>
		protected StringBuilder _from;
		/// <summary>
		/// ���[���̃w�b�_�ł��B
		/// </summary>
		protected StringBuilder _header;
		/// <summary>
		/// ���[���̖{���ł��B
		/// </summary>
		protected StringBuilder _body;
		/// <summary>
		/// �W�J�����Y�t�t�@�C�����ł��B
		/// </summary>
		protected StringBuilder _filename;
		/// <summary>
		/// �Y�t�t�@�C�����̃��X�g�ł��B
		/// </summary>
		protected string[] _filename_list = null;
		/// <summary>
		/// �����t�@�C���� ID �ł��B
		/// </summary>
		protected StringBuilder _id;
		/// <summary>
		/// �Y�t�t�@�C���W�J�p�o�b�t�@�ł��B
		/// </summary>
		protected byte[] _temp;
		/// <summary>
		/// �W�J���J�n�������ǂ����̃t���O�ł��B
		/// </summary>
		protected bool _attachment_flag = false;
		/// <summary>
		/// Dispose �������s�������ǂ����̃t���O�ł��B
		/// </summary>
		private bool _disposed = false;
		/// <summary>
		/// text/html �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		protected StringBuilder _html_file = null;
		/// <summary>
		/// �w�b�_�t�B�[���h���ł��B
		/// </summary>
		protected string _field_name = "";
		/// <summary>
		/// �w�b�_�t�B�[���h���e�i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _field = null;
		/// <summary>
		/// message/rfc822 �p�[�g��ۑ������t�@�C���̖��O�ł��B
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
		/// <c>Attachment</c>�N���X�̐V�K�C���X�^���X�����������܂��B
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
		/// <see cref="Attachment"/>�ɂ���Ďg�p����Ă��邷�ׂẴ��\�[�X��������܂��B
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// <see cref="Attachment"/>�ɂ���Ďg�p����Ă��邷�ׂẴ��\�[�X��������܂��B
		/// </summary>
		/// <param name="disposing">
		/// �}�l�[�W���\�[�X�ƃA���}�l�[�W���\�[�X�̗������������ꍇ��<c>true</c>�B
		/// �A���}�l�[�W���\�[�X�������������ꍇ��<c>false</c>�B
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
		/// �Y�t�t�@�C���W�J�Ŏg�p���Ă������\�[�X��������܂��B
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
		/// �w�b�_�i�[�p�o�b�t�@�̃T�C�Y�����肵�܂��B
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
		/// ���[���{���̍��v�T�C�Y���擾���܂��B
		/// </summary>
		/// <returns>���[���{���̍��v�T�C�Y</returns>
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
		/// �Y�t�t�@�C����W�J���ĕۑ����܂��B
		/// </summary>
		/// <exception cref="FormatException">
		///	<see cref="Add"/>�Ō��ƂȂ镶����i�w�b�_,�{��)���ǉ�����Ă��܂���B
		///	</exception>
		/// <exception cref="DirectoryNotFoundException">
		///	<see cref="Path"/>�Ŏw�肵���t�H���_�����݂��܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�W�J���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// �W�J���ƂȂ镪���t�@�C���f�[�^��������X�g�ɒǉ����܂��B
		/// </summary>
		/// <param name="header_body">�w�b�_�[�{�{���̕�����</param>
		/// <remarks>
		///	�w�b�_�Ɩ{���̊Ԃɂ͈�s�̋�s���K�v�ł��B
		/// </remarks>
		public void Add(string header_body)
		{
			_header_list.Add(header_body);
		}
		/// <summary>
		/// �W�J���ƂȂ镪���t�@�C���f�[�^��������X�g�ɒǉ����܂��B
		/// </summary>
		/// <param name="header">�w�b�_�[�̕�����</param>
		/// <param name="body">�{���̕�����</param>
		public void Add(string header, string body)
		{
			_header_list.Add(header);
			_body_list.Add(body);
		}
		/// <summary>
		/// �����t�@�C���� ID ���擾���܂��B
		/// </summary>
		/// <param name="header">�w�b�_�[�̕�����</param>
		/// <returns><see cref="NoAttachmentFile"/>�̏ꍇ��������Ă��Ȃ��Y�t�t�@�C���t�����[���ł��B
		/// 1 �ȏ�̏ꍇ�A�����t�@�C���̔ԍ��ƂȂ�܂��B
		/// </returns>
		public int GetId(string header)
		{
			_id = new StringBuilder(header.Length);
			return AttachmentFileStatus(header, _id, header.Length);
		}
		/// <summary>
		/// ���[���w�b�_����w��̃t�B�[���h�̓��e���擾���܂��B
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <returns>�t�B�[���h�̓��e
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
		/// ���[���w�b�_����w��̃t�B�[���h�̓��e���擾���܂��B
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <param name="header">�w�b�_</param>
		/// <returns>�t�B�[���h�̓��e
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
		/// MIME �w�b�_�t�B�[���h�̕�������f�R�[�h���܂�
		/// </summary>
		/// <param name="field">�t�B�[���h�̕�����</param>
		/// <returns>�f�R�[�h�����t�B�[���h���e</returns>
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
		/// ���[���w�b�_����w��̃w�b�_�t�B�[���h�̓��e���擾���A
		/// MIME �w�b�_�t�B�[���h�̃f�R�[�h���s���ĕԂ��܂�
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <returns>�擾�����f�R�[�h�ς݂̃t�B�[���h���e</returns>
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
		/// ���[���w�b�_����w��̃w�b�_�t�B�[���h�̓��e���擾���A
		/// MIME �w�b�_�t�B�[���h�̃f�R�[�h���s���ĕԂ��܂�
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <param name="header">�w�b�_</param>
		/// <returns>�擾�����f�R�[�h�ς݂̃t�B�[���h���e</returns>
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
		/// �Y�t�t�@�C�����̔z��ł��B
		/// </summary>
		///	<remarks>
		/// ���̃v���p�e�B�͌݊����̂��߂Ɏc���Ă���܂��B
		///	<see cref="GetFileNameList"/>�Ŕz����擾���Ďg�p����悤�ɂ��Ă��������B
		///	</remarks>
		/// <returns>�Y�t�t�@�C�����̔z��</returns>
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
		/// ����
		/// </summary>
		/// <value>���[������</value>
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
		/// ���[���̑��M�����̕�����ł��B
		/// </summary>
		/// <value>���[�����M����������</value>
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
		/// ���[���̍��o�l�ł��B
		/// </summary>
		/// <value>���[�����o�l</value>
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
		/// ���[���̖{���ł��B
		/// </summary>
		/// <value>���[���{��</value>
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
		/// �Y�t�t�@�C����ۑ�����t�H���_�ł��B
		/// </summary>
		/// <remarks>
		/// null (VB.Net �� nothing) �̏ꍇ�ۑ����܂���B
		/// </remarks>
		/// <value>�Y�t�t�@�C���ۑ��t�H���_</value>
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
		/// �����t�@�C���� ID �ł��B
		/// </summary>
		/// <value>�����t�@�C�� ID</value>
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
		/// �Y�t�t�@�C�����ł��B
		/// </summary>
		/// <remarks>
		/// �����̓Y�t�t�@�C��������ꍇ�A"," �ŋ�؂��Ċi�[����܂��B
		/// <see cref="Options.SplitChar"/>�ŋ�؂蕶����ύX�ł��܂��B
		/// </remarks>
		/// <value>�Y�t�t�@�C����</value>
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
		/// �Y�t�t�@�C�����̔z��ł��B
		/// </summary>
		///	<remarks>
		/// ���̃v���p�e�B�͌݊����̂��߂Ɏc���Ă���܂��B
		///	<see cref="GetFileNameList"/>�Ŕz����擾���Ďg�p����悤�ɂ��Ă��������B
		///	</remarks>
		/// <value>�Y�t�t�@�C�����̔z��</value>
		public string[] FileNameList
		{
			get
			{
				return _filename_list;
			}
		}
		/// <summary>
		/// �G���[�ԍ�
		/// </summary>
		/// <value>�G���[�ԍ�</value>
		public int ErrorCode
		{
			get
			{
				return _err;
			}
		}
		/// <summary>
		/// text/html �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		/// <value>�t�@�C����</value>
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
		/// ���[���̃w�b�_�ł��B
		/// </summary>
		/// <value>���[���̃w�b�_</value>
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
		/// �w�b�_�t�B�[���h�̓��e�ł��B
		/// </summary>
		/// <value>�w�b�_�t�B�[���h�̓��e</value>
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
		/// message/rfc822 �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		/// <value>�t�@�C����</value>
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
	/// nMail.DLL �ݒ�p�N���X
	/// </summary>
	public class Options
	{
		/// <summary>
		/// <c>Options</c>�N���X�̐V�K�C���X�^���X�����������܂��B
		/// </summary>
		public Options()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		protected const int MessageSize = 32768;

		/// <summary>
		/// �I�v�V�����F�^�C���A�E�g
		/// </summary>
		protected const int OptionTimeout = 0;
		/// <summary>
		/// �I�v�V�����F�w�b�_�T�C�Y
		/// </summary>
		protected const int OptionHeaderMax = 1;
		/// <summary>
		/// �I�v�V�����F�{���T�C�Y
		/// </summary>
		protected const int OptionBodyMax = 2;
		/// <summary>
		/// �I�v�V�����F�㏑�����[�h
		/// </summary>
		protected const int OptionAlreadyFile = 3;
		/// <summary>
		/// �I�v�V�����F�f�o�b�N���[�h
		/// </summary>
		protected const int OptionDebugMode = 4;
		/// <summary>
		/// �I�v�V�����F�ڑ����^�C���A�E�g
		/// </summary>
		protected const int OptionConnectTimeout = 5;
		/// <summary>
		/// �I�v�V�����F��؂蕶��
		/// </summary>
		protected const int OptionSplitChar = 6;
		/// <summary>
		/// �I�v�V�����F�ꎞ�x�~�T�C�Y
		/// </summary>
		protected const int OptionSuspendSize = 7;
		/// <summary>
		/// �I�v�V�����FSleep ����
		/// </summary>
		protected const int OptionSleepTime = 8;
		/// <summary>
		/// �I�v�V�����F�t�B�[���h�T�C�Y
		/// </summary>
		protected const int OptionFieldMax = 9;
		/// <summary>
		/// �I�v�V�����Ftext/html �p�[�g�ۑ�
		/// </summary>
		protected const int OptionSaveHtmlFile = 10;
		/// <summary>
		/// �I�v�V�����F�{���̃f�R�[�h
		/// </summary>
		protected const int OptionDecodeBody = 11;
		/// <summary>
		/// �I�v�V�����F�w�b�_�̃f�R�[�h
		/// </summary>
		protected const int OptionDecodeHeader = 12;
		/// <summary>
		/// �I�v�V�����F�Y�t�t�@�C�����i�[�o�b�t�@�T�C�Y
		/// </summary>
		protected const int OptionFileNameMax = 13;
		/// <summary>
		/// �I�v�V�����F�Y�t�t�@�C�����Ɏg�p�ł��Ȃ�������u�������镶��
		/// </summary>
		protected const int OptionChangeChar = 14;
		/// <summary>
		/// �I�v�V�����F�Y�t�t�@�C������u�����������
		/// </summary>
		protected const int OptionChangeCharMode = 15;
		/// <summary>
		/// �I�v�V�����Fmessage/rfc822 �p�[�g�ۑ�
		/// </summary>
		protected const int OptionSaveRfc822File = 16;

		/// <summary>
		/// �����t�@�C�����㏑�����܂��B
		/// </summary>
		public const int FileOverwrite = 0;
		/// <summary>
		/// �t�@�C�����̖���(�g���q�̑O)�ɐ�����ǉ����ĕۑ����܂��B
		/// "file.dat" �����݂���ꍇ�A"file1.dat" �Ƃ������O�ŕۑ����܂��B
		/// </summary>
		public const int FileRename = 1;
		/// <summary>
		/// �t�@�C�����쐬�����G���[�Ƃ��܂��B
		/// </summary>
		public const int FileAlreadyError = 2;

		/// <summary>
		/// �G���[���b�Z�[�W���擾���܂��B
		/// </summary>
		protected const int MessageError = 0;
		/// <summary>
		/// �f�o�b�O�p�̃T�[�o�Ƃ̌�M��������擾���܂��B
		/// </summary>
		protected const int MessageDebug = 1;

		/// <summary>
		/// �f�o�b�N���[�h���L���ł��B
		/// </summary>
		public const int DebugStart = 1;
		/// <summary>
		/// �f�o�b�N���[�h�������ł��B(�����l)
		/// </summary>
		public const int DebugEnd = 0;

		/// <summary>
		/// �ڑ��^�C���A�E�g���L���ł��B
		/// </summary>
		public const int ConnectTimeoutOn = 1;
		/// <summary>
		/// �ڑ��^�C���A�E�g�������ł��B(�����l)
		/// </summary>
		public const int ConnectTimeoutOff = 0;

		/// <summary>
		/// text/html �p�[�g���t�@�C���ɕۑ����܂��B
		/// </summary>
		public const int SaveHtmlFileOn = 1;
		/// <summary>
		/// text/html �p�[�g���t�@�C���ɕۑ����܂���B(�����l)
		/// </summary>
		public const int SaveHtmlFileOff = 0;

		/// <summary>
		/// �{���͑S�ăf�R�[�h���܂��B(�����l)
		/// </summary>
		public const int DecodeBodyOn = 0;
		/// <summary>
		/// �{���͑S�ăf�R�[�h���܂���B�{���̃e�L�X�g�� iso-2022-jp �̂܂܂ƂȂ�܂��B
		/// </summary>
		public const int DecodeBodyAllOff = 1;
		/// <summary>
		/// �e�L�X�g�p�[�g�� Content-Transfer-Encoding �w��̃G���R�[�h�̂݃f�R�[�h���܂���B
		/// </summary>
		public const int DecodeBodyTextOff = 2;
		/// <summary>
		/// �w�b�_�͑S�ăf�R�[�h���܂��B(�����l)
		/// </summary>
		public const int DecodeHeaderOn = 0;
		/// <summary>
		/// �w�b�_�͑S�ăf�R�[�h���܂���B
		/// </summary>
		public const int DecodeHeaderOff = 1;

		/// <summary>
		/// �g�p�s�ȕ����Ƌ�؂蕶����u�������܂��B(�����l)
		/// </summary>
		public const int ChangeSplitChar = 0;
		/// <summary>
		/// ���p�X�y�[�X��u�������܂��B
		/// </summary>
		public const int ChangeHalfSpace = 1;
		/// <summary>
		/// �S�p�X�y�[�X��u�������܂��B
		/// </summary>
		public const int ChangeFullSpace = 2;
		/// <summary>
		/// ���p�E�S�p�X�y�[�X��u�������܂��B
		/// </summary>
		public const int ChangeAllSpace = 3;
		/// <summary>
		/// ��؂蕶����u�������܂���B
		/// </summary>
		public const int ChangeNoSplitChar = 4;

		/// <summary>
		/// message/rfc822 �p�[�g���܂Ƃ߂ăt�@�C���ɕۑ����܂��B
		/// </summary>
		public const int SaveRfc822FileAllOn = 2;
		/// <summary>
		/// message/rfc822 �p�[�g�̃e�L�X�g�������t�@�C���ɕۑ����܂��B
		/// </summary>
		public const int SaveRfc822FileBodyOn = 1;
		/// <summary>
		/// message/rfc822 �p�[�g���t�@�C���ɕۑ����܂���B
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
		/// �f�o�b�N���[�h���J�n���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="EndDebug"/>���Ăяo������� <see cref="DebugMessage"/>
		/// ���Q�Ƃ��邱�Ƃɂ���ăT�[�o�Ƃ̌�M��������擾�ł��܂��B
		/// </remarks>
		public static void StartDebug()
		{
			SetOption(OptionDebugMode, DebugStart);
		}

		/// <summary>
		/// �f�o�b�N���[�h���I�����܂��B
		/// </summary>
		/// <remarks>
		/// �Ăяo����� <see cref="DebugMessage"/> ���Q�Ƃ��邱�Ƃɂ���ăT�[�o
		/// �Ƃ̌�M��������擾�ł��܂��B
		/// </remarks>
		public static void EndDebug()
		{
			SetOption(OptionDebugMode, DebugEnd);
		}

		/// <summary>
		/// �ڑ����^�C���A�E�g�ݒ��L���ɂ��܂��B
		/// </summary>
		public static void EnableConnectTimeout()
		{
			SetOption(OptionConnectTimeout, ConnectTimeoutOn);
		}

		/// <summary>
		/// �ڑ����^�C���A�E�g�ݒ�𖳌��ɂ��܂��B(�����l)
		/// </summary>
		public static void DisableConnectTimeout()
		{
			SetOption(OptionConnectTimeout, ConnectTimeoutOff);
		}
		/// <summary>
		/// text/html �p�[�g���t�@�C���ɕۑ����܂��B
		/// </summary>
		public static void EnableSaveHtmlFile()
		{
			SetOption(OptionSaveHtmlFile, SaveHtmlFileOn);
		}
		/// <summary>
		/// text/html �p�[�g���t�@�C���ɕۑ����܂���B(�����l)
		/// </summary>
		public static void DisableSaveHtmlFile()
		{
			SetOption(OptionSaveHtmlFile, SaveHtmlFileOff);
		}
		/// <summary>
		/// �{�����f�R�[�h���܂��B(�����l)
		/// </summary>
		public static void EnableDecodeBody()
		{
			SetOption(OptionDecodeBody, DecodeBodyOn);
		}
		/// <summary>
		/// �{�����f�R�[�h���܂���B�{���̃e�L�X�g�� iso-2022-jp �̂܂܂ƂȂ�܂��B
		/// </summary>
		public static void DisableDecodeBodyAll()
		{
			SetOption(OptionDecodeBody, DecodeBodyAllOff);
		}
		/// <summary>
		/// �e�L�X�g�p�[�g�� Content-Transfer-Encoding �w��̃G���R�[�h�̂݃f�R�[�h���܂���B
		/// </summary>
		public static void DisableDecodeBodyText()
		{
			SetOption(OptionDecodeBody, DecodeBodyTextOff);
		}
		/// <summary>
		/// �w�b�_���f�R�[�h���܂��B(�����l)
		/// </summary>
		public static void EnableDecodeHeader()
		{
			SetOption(OptionDecodeHeader, DecodeHeaderOn);
		}
		/// <summary>
		/// �w�b�_���f�R�[�h���܂���B
		/// </summary>
		public static void DisableDecodeHeader()
		{
			SetOption(OptionDecodeHeader, DecodeHeaderOff);
		}
		/// <summary>
		/// message/rfc822 �p�[�g���܂Ƃ߂ăt�@�C���ɕۑ����܂��B
		/// </summary>
		public static void EnableSaveRfc822FileAll()
		{
			SetOption(OptionSaveRfc822File, SaveRfc822FileAllOn);
		}
		/// <summary>
		/// message/rfc822 �p�[�g�̃e�L�X�g�������t�@�C���ɕۑ����܂��B
		/// </summary>
		public static void EnableSaveRfc822FileBody()
		{
			SetOption(OptionSaveRfc822File, SaveRfc822FileBodyOn);
		}
		/// <summary>
		/// message/rfc822 �p�[�g���t�@�C���ɕۑ����܂���B�i�����l�j
		/// </summary>
		public static void DisableSaveRfc822File()
		{
			SetOption(OptionSaveRfc822File, SaveRfc822FileOff);
		}
		/// <summary>
		/// �w�肳�ꂽ�t�@�C����Y�t�������[���𑗐M����ꍇ�̋x�~�񐔂𓾂�
		/// </summary>
		/// <param name="file_name">�Y�t�t�@�C����</param>
		/// <exception cref="FileNotFoundException">
		/// file_name �Őݒ肳��Ă���t�@�C�������݂��܂���B
		/// </exception>
		/// <value>�w�肳�ꂽ�t�@�C����Y�t�������[���𑗐M����ꍇ�̋x�~�񐔂𓾂�</value>
		public static int SuspendCount(String file_name)
		{
			if(!File.Exists(file_name)) {
				throw new FileNotFoundException(file_name);
			}
			return GetSuspendNumber(file_name);
		}

		/// <summary>
		/// �o�[�W�����̐��l�ł��B
		/// </summary>
		/// <remarks>
		/// nMail.DLL �̃o�[�W�����̐��l�ł��B
		/// Version 1.23 �̏ꍇ 123 �ƂȂ�܂��B
		/// </remarks>
		/// <value>�o�[�W�������l</value>
		public static int VersionInt
		{
			get
			{
				return GetVersion();
			}
		}
		/// <summary>
		/// �o�[�W�����̕�����ł��B
		/// </summary>
		/// <remarks>
		/// nMail.DLL �̃o�[�W�����̕�����ł��B
		/// Version 1.23 �̏ꍇ "1.23" �ƂȂ�܂��B
		/// </remarks>
		/// <value>�o�[�W����������</value>
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
		/// �G���[���b�Z�[�W�ł��B
		/// </summary>
		/// <remarks>
		/// �G���[�������ɃT�[�o����Ԃ��Ă����G���[���b�Z�[�W�ł��B
		/// </remarks>
		/// <value>�G���[���b�Z�[�W</value>
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
		/// �f�o�b�N�p������ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="StartDebug"/>���Ăяo������Ƀ��[���̑���M�������s���A
		/// <see cref="EndDebug"/>���Ăяo������ɂ��̃v���p�e�B���Q�Ƃ��邱��
		/// �ɂ���ăT�[�o�Ƃ̌�M��������擾���邱�Ƃ��ł��܂��B
		/// </remarks>
		/// <value>�f�o�b�N�p������</value>
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
		/// �����^�C���A�E�g�l�ł��B
		/// </summary>
		/// <remarks>
		/// ���̐ݒ�l��蒷�����ԃT�[�o���牞���������ꍇ�A
		/// �^�C���A�E�g�G���[�Ƃ��ď�����Ԃ��܂��B
		/// �P�ʂ� ms �ł��B
		/// �ڑ����̃^�C���A�E�g�͒ʏ�͖����ł��B
		/// �L���ɂ������ꍇ�A<see cref="EnableConnectTimeout"/>
		/// ���Ăяo���Ă��������B
		/// </remarks>
		/// <value>�����^�C���A�E�g�l</value>
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
		/// �w�b�_�o�b�t�@�T�C�Y�ł��B
		/// </summary>
		/// <remarks>
		/// ���[����M��Y�t�t�@�C���W�J���Ƀw�b�_�f�[�^���i�[����
		/// �o�b�t�@�̃T�C�Y�ł��B
		/// <see cref="Pop3"/>�N���X�Ń��[���̎�M���s���ꍇ�A������
		/// �o�b�t�@�T�C�Y�̐ݒ���s���܂��̂ŁA���̃v���p�e�B��
		/// �g�p����K�v�͂���܂���B
		/// </remarks>
		/// <value>�w�b�_�o�b�t�@�T�C�Y</value>
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
		/// �{���o�b�t�@�T�C�Y�ł��B
		/// </summary>
		/// <remarks>
		/// ���[����M��Y�t�t�@�C���W�J���Ƀw�b�_�f�[�^���i�[����
		/// �{���̃T�C�Y�ł��B
		/// <see cref="Pop3"/>�N���X�Ń��[���̎�M���s���ꍇ�A������
		/// ���[���T�C�Y�ɉ������o�b�t�@�T�C�Y�̎w����s���܂��̂ŁA
		/// ���̃v���p�e�B���g�p����K�v�͂���܂���B
		/// </remarks>
		/// <value>�{���o�b�t�@�T�C�Y</value>
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
		/// �Y�t�t�@�C���ۑ����ɁA�w��t�H���_�ɓ����t�@�C�����������ꍇ�̏������@�ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="FileOverwrite"/>�����t�@�C�����㏑�����܂��B
		/// <see cref="FileRename"/>�t�@�C�����̖���(�g���q�̑O)�ɐ�����ǉ����ĕۑ����܂��B
		/// <see cref="FileAlreadyError"/>�t�@�C�����쐬�����G���[�Ƃ��܂��B
		/// </remarks>
		/// <value>�Y�t�t�@�C���ۑ����̓����t�@�C���������@</value>
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
		/// �f�o�b�N���[�h
		/// </summary>
		/// <remarks>
		/// <see cref="DebugStart"/>�f�o�b�N���[�h���L���ł��B
		/// <see cref="DebugEnd"/>�f�o�b�N���[�h�������ł��B
		/// </remarks>
		/// <value>�f�o�b�N���[�h</value>
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
		/// �T�[�o�ڑ����̃^�C���A�E�g���L�����ǂ����̐ݒ�ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="ConnectTimeoutOn"/>�ڑ��^�C���A�E�g���L���ł��B
		/// <see cref="ConnectTimeoutOff"/>�ڑ��^�C���A�E�g�������ł��B
		/// </remarks>
		/// <value>�T�[�o�ڑ������L�����ǂ����̐ݒ�l</value>
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
		/// �Y�t�t�@�C���̋�؂蕶���ł��B
		/// </summary>
		/// <remarks>
		/// �Y�t�t�@�C������������ꍇ�̋�؂蕶���ł��B
		///	�f�t�H���g�l�� , �ł��B
		/// </remarks>
		/// <value>�Y�t�t�@�C����؂蕶��</value>
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
		/// ����M�̍ۂɈ�U������߂��f�[�^�T�C�Y�ł��B
		/// </summary>
		/// <remarks>
		/// �Y�t�t�@�C��������ꍇ�A�����Ŏw�肵���T�C�Y�̃f�[�^��
		/// ����M����ƁA��U������߂��܂��B
		/// </remarks>
		/// <value>����M�̍ۂɈ�U������߂��f�[�^�T�C�Y</value>
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
		/// �T�[�o����̕ԓ��҂��̍ۂɓ���� Sleep() �̎��Ԃł��B
		/// </summary>
		/// <value>�T�[�o����̕ԓ��҂��̍ۂɓ���� Sleep() �̎���</value>
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
		/// �t�B�[���h�o�b�t�@�̃T�C�Y�ł��B
		/// </summary>
		/// <value>�t�B�[���h�o�b�t�@�̃T�C�Y</value>
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
		/// HTML ���[���� text/html �p�[�g���t�@�C����ۑ����邩�ǂ����̐ݒ�ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="SaveHtmlFileOff"/>text/html �p�[�g���t�@�C���ɕۑ����Ȃ�(�����l)
		/// <see cref="SaveHtmlFileOn"/>text/html �p�[�g���t�@�C���ɕۑ�����
		/// Attachment �N���X���g���� text/html �p�[�g���t�@�C���ɕۑ�����ꍇ�A
		/// <see cref="DisableDecodeBodyText()"/> �܂��� <see cref="DisableDecodeBodyAll()"/> ���Ăяo������� <see cref="Pop3.GetMail"/> �œǂݏo����
		/// �{���f�[�^���g�p���A<see cref="Attachment.Save()"/> �̑O�� <see cref="EnableDecodeBody()"/>
		/// �����s���Ă����K�v������܂��B
		/// </remarks>
		/// <value>text/html �p�[�g���t�@�C���ɕۑ����邩�ǂ����̐ݒ�l</value>
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
		/// �{���̃f�R�[�h�̐ݒ�ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="DecodeBodyOn"/>�S�ăf�R�[�h���܂��B(�����l)
		/// <see cref="DecodeBodyAllOff"/>�S�ăf�R�[�h���܂���B�{���̃e�L�X�g�� iso-2022-jp �̂܂܂ƂȂ�܂��B
		/// <see cref="DecodeBodyTextOff"/>�e�L�X�g�p�[�g�� Content-Transfer-Encoding �w��̃G���R�[�h�̂݃f�R�[�h���܂���B
		/// </remarks>
		/// <value>�{�����f�R�[�h���邩�ǂ����̐ݒ�l</value>
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
		/// �w�b�_�̃f�R�[�h�̐ݒ�ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="DecodeHeaderOn"/>�S�ăf�R�[�h���܂��B(�����l)
		/// <see cref="DecodeHeaderOff"/>�S�ăf�R�[�h���܂���B
		/// </remarks>
		/// <value>�w�b�_���f�R�[�h���邩�ǂ����̐ݒ�l</value>
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
		/// �Y�t�t�@�C�����i�[�o�b�t�@�T�C�Y�ł��B
		/// </summary>
		/// <remarks>
		/// �Y�t�t�@�C�������i�[����o�b�t�@�̃T�C�Y�ł��B
		/// <see cref="Pop3"/>��<see cref="Imap4"/>�N���X�Ń��[���̎�M���s������A
		/// <see cref="Attachment"/>�N���X�œY�t�t�@�C���̓W�J���s���ꍇ�A
		/// �����Ńo�b�t�@�T�C�Y�̐ݒ���s���܂��̂ŁA
		/// ���̃v���p�e�B���g�p����K�v�͂���܂���B
		/// </remarks>
		/// <value>�Y�t�t�@�C�����i�[�o�b�t�@�T�C�Y</value>
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
		/// �Y�t�t�@�C�����Ɏg�p�ł��Ȃ�������u�������镶��
		/// </summary>
		/// <remarks>
		/// �Y�t�t�@�C�����Ɏg�p�ł��Ȃ�������u�������镶���ł��B
		/// �f�t�H���g�l�� _ �ł��B
		/// </remarks>
		/// <value>�u����������</value>
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
		/// �Y�t�t�@�C������u�����������
		/// </summary>
		/// <remarks>
		/// �Y�t�t�@�C������u������������ł��B
		/// �f�t�H���g�l��<see cref="ChangeSplitChar"/>�ŁA
		/// Windows �Ńt�@�C�����Ɏg�p�ł��Ȃ������ƁA��؂蕶����
		/// �g�p���Ă��镶����u�������܂��B
		/// </remarks>
		/// <value>�u����������</value>
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
		/// message/rfc822 �p�[�g���t�@�C���ɕۑ����邩�ǂ����̐ݒ�ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="SaveRfc822FileOff"/>message/rfc822 �p�[�g���t�@�C���ɕۑ����Ȃ��B(�����l)
		/// <see cref="SaveRfc822FileBodyOn"/>message/rfc822 �p�[�g�̃e�L�X�g�������t�@�C���ɕۑ�����B
		/// <see cref="SaveRfc822FileAllOn"/>message/rfc822 �p�[�g��S�ăt�@�C���ɕۑ�����B
		/// SaveRfc822FileAllOn �� message/rfc822 �p�[�g��S�ăt�@�C���ɕۑ������ꍇ�A
		/// Attachment �N���X���g���ēY�t�t�@�C�������o�������ł��܂��B
		/// </remarks>
		/// <value>message/rfc822 �p�[�g���t�@�C���ɕۑ����邩�ǂ����̐ݒ�l</value>
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
	/// IMAP4rev1 ���[����M�N���X
	/// </summary>
	/// <remarks>
	/// <example>
	/// <para>
	///	���[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)�� MIME �\�����擾���A�e�L�X�g�ł���Ζ{����\���A�Y�t�t�@�C���ł���� z:\temp �ɕۑ����܂��B
	/// <code lang="cs">
	///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
	///		try {
	///			imap.Connect();
	///			imap.Authenticate("imap4_id", "password");
	///			imap.SelectMailBox("inbox");
	///			// MIME �\�����擾
	///			imap.GetMimeStructure(no);
	///			nMail.Imap4.MimeStructureStatus [] list = imap4.GetMimeStructureList()
	///			foreach(nMail.Imap4.MimeStructureStatus mime in list)
	///			{
	///				// �e�L�X�g�ł���Ζ{�����擾���ĕ\��
	///				if(string.Compare(mime.Type, "text", true) == 0 &amp;&amp; string.Compare(mime.SubType, "plain", true) == 0)
	///				{
	///					// �{���擾
	///					imap.GetMimePart(no, mime.PartNo);
	///					MessageBox.Show(imap.Body);
	///				}
	///				else if(string.Compare(mime.Type, "multipart", true) != 0)
	///				{
	///					// �Y�t�t�@�C����ۑ�
	///					imap.SaveMimePart(no, mime.PartNo, @"z:\temp\" + mime.FileName);
	///				}
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
	///	Try
	///		imap.Connect()
	///		imap.Authenticate("imap4_id", "password")
	///		imap.SelectMailBox("inbox")
	///		' MIME �\�����擾
	///		imap.GetMimeStructure(no)
	///		Dim list As nMail.Imap4.MimeStructureStatus() = imap4.GetMimeStructureList()
	///		For Each mime As nMail.Imap4.MimeStructureStatus In list
	///			' �e�L�X�g�ł���Ζ{�����擾���ĕ\��
	///			If (String.Compare(mime.Type, "text", True) = 0) And (String.Compare(mime.SubType, "plain", True) = 0) Then
	///				' �{���擾
	///				imap.GetMimePart(no, mime.PartNo)
	///				MessageBox.Show(imap.Body)
	///			Else If String.Compare(mime.Type, "multipart", true) &lt;&gt; 0 Then
	///				' �Y�t�t�@�C����ۑ�
	///				imap.SaveMimePart(no, mime.PartNo, "z:\temp\" + mime.FileName)
	///			End If
	///		Next
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		imap.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// ���[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)���擾���A�����Ɩ{����\������B
	/// ��ق� Attachment �N���X�œY�t�t�@�C����W�J����ꍇ���� text/html �p�[�g���t�@�C���ɕۑ��������ꍇ�A
	/// <see cref="Options.DisableDecodeBodyText()"/> �������� <see cref="Options.DisableDecodeBodyAll()"/>���Ă�ł���<see cref="GetMail"/>�Ŏ擾�����w�b�_����і{���f�[�^���g�p����K�v������܂��B
	/// <code lang="cs">
	///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
	///		try {
	///			imap.Connect();
	///			imap.Authenticate("imap4_id", "password");
	///		    imap.SelectMailBox("inbox");
	///			imap.GetMail(no);
	///			MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}\r\n{2:s}", no, imap.Subject, imap.Body));
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
	/// Try
	/// 	imap.Connect()
	/// 	imap.Authenticate("imap4_id", "password")
	///		imap.SelectMailBox("inbox")
	///		imap.GetMail(no)
	///		MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}" + ControlChars.CrLf + "{2:s}", no, imap.Subject, imap.Body))
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		imap.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// STARTTLS ���g�p���ă��[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)���擾���A�����Ɩ{����\������B
	/// <code lang="cs">
	///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
	///		try {
	///			imap.SSL = nMail.Imap4.STARTTLS;
	///			imap.Connect();				// STARTTLS �̏ꍇ�ʏ�� IMAP4 �|�[�g�ԍ��Őڑ�
	///			imap.Authenticate("imap4_id", "password");
	///		    imap.SelectMailBox("inbox");
	///			imap.GetMail(no);
	///			MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}\r\n{2:s}", no, imap.Subject, imap.Body));
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
	/// Try
	/// 	imap.SSL = nMail.Imap4.STARTTLS
	/// 	imap.Connect()					' STARTTLS �̏ꍇ�ʏ�� IMAP4 �|�[�g�ԍ��Őڑ�
	/// 	imap.Authenticate("imap4_id", "password")
	///		imap.SelectMailBox("inbox")
	///		imap.GetMail(no)
	///		MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}" + ControlChars.CrLf + "{2:s}", no, imap.Subject, imap.Body))
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		imap.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	/// ���[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)���擾���A�����Ɩ{����\������B�Y�t�t�@�C���� z:\temp �ɕۑ�����B
	/// text/html �p�[�g���t�@�C���ɕۑ�����ꍇ�A<see cref="GetMail"/> �̑O�� <see cref="Options.EnableSaveHtmlFile()"/> ���Ă�ł����K�v������܂��B
	/// �ۑ����� text/html �p�[�g�̃t�@�C������ <see cref="HtmlFile"/> �Ŏ擾�ł��܂��B
	/// <code lang="cs">
	///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
	///		try {
	///			imap.Connect();
	///			imap.Authenticate("imap4_id", "password");
	///		    imap.SelectMailBox("inbox");
	///			imap.Path = @"z:\temp";
	///			imap.GetMail(no);
	///			MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}\r\n{2:s}", no, imap.Subject, imap.Body));
	///			string [] file_list = imape.GetFileNameList();
	///			if(file_list.Length == 0)
	///			{
	///				MessageBox.Show("�t�@�C���͂���܂���");
	///			}
	///			else
	///			{
	///				foreach(string name in file_list)
	///				{
	///					MessageBox.Show(String.Format("�Y�t�t�@�C����:{0:s}", name));
	///				}
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
	/// Try
	/// 	imap.Connect()
	/// 	imap.Authenticate("imap4_id", "password")
	///		imap.SelectMailBox("inbox")
	///		imap.Path = "z:\temp"
	///		imap.GetMail(no)
	///		MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}" + ControlChars.CrLf + "{2:s}", no, imap.Subject, imap.Body))
	///		Dim file_list As String() = imap.GetFileNameList()
	///		If file_list.Length = 0 Then
	///			MessageBox.Show("�t�@�C���͂���܂���")
	///		Else
	///			For Each name As String In file_list
	///				MessageBox.Show(String.Format("�Y�t�t�@�C����:{0:s}", name))
	///			Next fno
	///		End If
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
	///	Finally
	///		imap.Dispose()
	///	End Try
	/// </code>
	/// </para>
	/// <para>
	///	�ꎞ�x�~�@�\���g���ă��[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)���擾���A�����Ɩ{����\������B�Y�t�t�@�C���� z:\temp �ɕۑ�����B
	/// <code lang="cs">
	///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
	///		try {
	///			imap.Connect();
	///			imap.Authenticate("imap4_id", "password");
	///			imap.SelectMailBox("inbox");
	///			// ���������̋x�~�񐔂𓾂�
	///			int count = imap.GetSize(no) / (nMail.Options.SuspendSize * 1024) + 1;
	///			imap.Path = @"z:\temp";
	///			imap.GetMail(no);
	///			imap.Flag = nMail.Imap4.SuspendAttachmentFile;
	///			imap.GetMail(no);
	///			imap.Flag = nMail.Imap4.SuspendNext;
	///			while(imap.ErrorCode == nMail.Imap4.ErrorSuspendAttachmentFile)
	///			{
	///				imap.GetMail(no);
	///				// �v���O���X�o�[��i�߂铙�̏���
	///				Application.DoEvents();
	///			}
	///			MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}\r\n{2:s}", no, imap.Subject, imap.Body));
	///			string [] file_list = imape.GetFileNameList();
	///			if(file_list.Length == 0)
	///			{
	///				MessageBox.Show("�t�@�C���͂���܂���");
	///			}
	///			else
	///			{
	///				foreach(string name in file_list)
	///				{
	///					MessageBox.Show(String.Format("�Y�t�t�@�C����:{0:s}", name));
	///				}
	///			}
	///		}
	///		catch(nMail.nMailException nex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
	///		}
	///		catch(Exception ex)
	///		{
	///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
	///		}
	///	}
	/// </code>
	/// <code lang="vbnet">
	///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
	///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
	///	Try
	///		Dim count As Integer
	///
	///		imap.Connect()
	///		imap.Authenticate("imap4_id", "password")
	///		imap.SelectMailBox("inbox");
	///		' ���������̋x�~�񐔂𓾂�
	///		count = imap.GetSize(no) \ (nMail.Options.SuspendSize * 1024) + 1
	///		imap.Path = "z:\temp"
	///		imap.Flag = nMail.Imap4.SuspendAttachmentFile
	///		imap.GetMail(no)
	///		imap.Flag = nMail.Imap4.SuspendNext
	///		Do While imap.ErrorCode = nMail.Imap4.ErrorSuspendAttachmentFile
	///			imap.GetMail(no)
	///			' �v���O���X�o�[��i�߂铙�̏���
	///			Application.DoEvents()
	///		Loop
	///		MessageBox.Show(String.Format("���[���ԍ�:{0:d} ����:{1:s}" + ControlChars.CrLf + "{2:s}", no, imap.Subject, imap.Body))
	///		Dim file_list As String() = imap.GetFileNameList()
	///		If file_list.Length = 0 Then
	///			MessageBox.Show("�t�@�C���͂���܂���")
	///		Else
	///			For Each name As String In file_list
	///				MessageBox.Show(String.Format("�Y�t�t�@�C����:{0:s}", name))
	///			Next fno
	///		End If
	///	Catch nex As nMail.nMailException
	///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
	///	Catch ex As Exception
	///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
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
		/// �w�b�_�̈�̃T�C�Y�ł��B
		/// </summary>
		protected const int HeaderSize = 32768;
		/// <summary>
		/// �p�X������̃T�C�Y�ł��B
		/// </summary>
		protected const int MaxPath = 32768;
		/// <summary>
		/// �\�P�b�g�G���[�������͖��ڑ���Ԃł��B�l�� -1 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Connect"/>���Ăяo������<see cref="GetMail"/>�����A</para>
		/// <para>�Ăяo������A�Ȃ�炩�̗��R�Őڑ����ؒf�����Ƃ��̃G���[�ƂȂ�܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorSocket = -1;
		/// <summary>
		/// �F�؃G���[�ł��B�l�� -2 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Authenticate"/>�Ăяo���ŔF�؂Ɏ��s�����ꍇ</para>
		/// <para>���̃G���[�ƂȂ�܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorAuthenticate = -2;
		/// <summary>
		/// �ԍ��Ŏw�肳�ꂽ���[�������݂��܂���B�l�� -3 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>�Ŏw�肵���ԍ��̃��[�������݂��Ȃ��Ƃ��̃G���[�ƂȂ�܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorInvalidNo = -3;
		/// <summary>
		/// �^�C���A�E�g�G���[�ł��B�l�� -4 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Options.Timeout"/>�Ŏw�肵���l��蒷�����ԃT�[�o���牞����</para>
		/// <para>�Ȃ��ꍇ�A���̃G���[���������܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorTimeout = -4;
		/// <summary>
		/// �Y�t�t�@�C�����J���܂���B�l�� -5 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Path"/>�Ŏw�肵���t�H���_�ɓY�t�t�@�C�����������߂Ȃ�</para>
		/// <para>�Ȃ��ꍇ�A���̃G���[���������܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorFileOpen = -5;
		/// <summary>
		/// �Y�t�t�@�C���Ɠ����̃t�@�C�����t�H���_�ɑ��݂��܂��B�l�� -7 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="nMail.Options.AlreadyFile"/> �� <see cref="nMail.Options.FileAlreadyError"/></para>
		/// <para>��ݒ肵�A<see cref="Path"/>�Ŏw�肵���t�H���_�ɓY�t�t�@�C���Ɠ������O�̃t�@�C����</para>
		/// <para>����ꍇ�ɂ��̃G���[���������܂��B</para>
		/// <para><see cref="nMailException.ErrorCode"/>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorFileAlready = -7;
		/// <summary>
		/// �T�[�o���w�肵���F�،`���ɑΉ����Ă��܂���B�l�� -8 �ł��B
		/// </summary>
		public const int ErrorAuthenticateNoSupport = -8;
		/// <summary>
		/// �������m�ۃG���[�ł��B�l�� -9 �ł��B
		/// </summary>
		/// <remarks>
		/// �����ŕ����R�[�h��ϊ����邽�߂̃��������m�ۂł��܂���ł����B
		/// </remarks>
		public const int ErrorMemory = -9;
		/// <summary>
		/// ���̑��̃G���[�ł��B�l�� -10 �ł��B
		/// </summary>
		public const int ErrorEtc = -10;
		/// <summary>
		/// �p�����[�^������������܂���B�l�� -11 �ł��B
		/// </summary>
		/// <remarks>
		/// �p�����[�^������������܂���B
		/// </remarks>
		public const int ErrorInvalidParameter = -11;
		/// <summary>
		/// IMAP4 �T�[�o�����G���[�B�l�� -12 �ł��B
		/// </summary>
		/// <remarks>
		/// IMAP4 �T�[�o�� NO ��Ԃ��܂����B
		/// </remarks>
		public const int ErrorResponseNo = -12;
		/// <summary>
		/// IMAP4 �T�[�o�����G���[�B�l�� -13 �ł��B
		/// </summary>
		/// <remarks>
		/// IMAP4 �T�[�o�� Bad ��Ԃ��܂����B
		/// </remarks>
		public const int ErrorResponseBad = -13;
		/// <summary>
		/// ���O��Ԃ����݂��܂���B�l�� -14 �ł��B
		/// </summary>
		public const int ErrorNoNameSpace = -14;
		/// <summary>
		/// �Y�t�t�@�C����M���ł܂��c�肪�����Ԃł��B�l�� -20 �ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="Flag"/>��<see cref="SuspendAttachmentFile"/>�܂���</para>
		/// <para><see cref="SuspendNext"/>���w�肵<see cref="GetMail"/></para>
		/// <para>���Ăяo�����ꍇ�A�܂����[���̎c�肪����ꍇ�A<see cref="nMailException.ErrorCode"/></para>
		/// <para>�ɐݒ肳��܂��B</para>
		/// </remarks>
		public const int ErrorSuspendAttachmentFile = -20;
		/// <summary>
		/// �Y�����鍀�ڂ�����܂���B�l�� -21 �ł��B
		/// </summary>
		/// <remarks>
		/// �Y�����鍀�ڂ͑��݂��܂���B
		/// </remarks>
		public const int ErrorNoResult = -21;
		/// <summary>
		/// �F�؂� PLAIN ���g�p���܂��B
		/// </summary>
		public const int AuthPlain = 1;
		/// <summary>
		/// �F�؂� LOGIN ���g�p���܂��B
		/// </summary>
		public const int AuthLogin = 2;
		/// <summary>
		/// �F�؂� CRAM MD5 ���g�p���܂��B
		/// </summary>
		public const int AuthCramMd5 = 4;
		/// <summary>
		/// �F�؂� DIGEST MD5 ���g�p���܂��B
		/// </summary>
		public const int AuthDigestMd5 = 8;
		/// <summary>
		/// ���s����
		/// </summary>
		/// <para>�e�탁�\�b�h��������Ԃ����ꍇ�A<see cref="nMailException.ErrorCode"/>�ɐݒ肳���l�ł��B</para>
		public const int Success = 1;
		/// <summary>
		/// �Y�t�t�@�C����M�ňꎞ�x�~����̈��ڂɎw�肵�܂��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>�̑O�ɁA<see cref="Flag"/>�ɐݒ肵�Ă����܂��B</para>
		/// </remarks>
		public const int SuspendAttachmentFile = 4;
		/// <summary>
		/// �Y�t�t�@�C����M�ňꎞ�x�~����̓��ڈȍ~�Ɏw�肵�܂��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMail"/>�̑O�ɁA<see cref="Flag"/>�ɐݒ肵�Ă����܂��B</para>
		/// </remarks>
		public const int SuspendNext = 8;
		/// <summary>
		/// ���[���ǉ����ɓ����t�B�[���h�� nMail.DLL ���Ő������܂��B
		/// </summary>
		public const int AddDateField = 1;
		/// <summary>
		/// ���[���ǉ�����Message-ID �t�B�[���h�� nMail.DLL ���Ő������܂��B
		/// </summary>
		public const int AddMessageId = 2;
		/// <summary>
		/// MIME �p�[�g�ۑ��ň����̃p�X�Ńt�@�C�������w�肵�܂��B
		/// </summary>
		private const int MimeSaveAsFile = 0x0800;
		/// <summary>
		/// MIME �p�[�g�擾�Ŋ��ǃt���O�𗧂Ă܂���B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMimePart"/>�̑O�ɁA<see cref="MimeFlag"/>�ɐݒ肵�Ă����܂��B</para>
		/// </remarks>
		public const int MimePeek = 0x1000;
		/// <summary>
		/// MIME �p�[�g�擾�Ńf�R�[�h���܂���B
		/// </summary>
		/// <remarks>
		/// <para><see cref="GetMimePart"/>�̑O�ɁA<see cref="MimeFlag"/>�ɐݒ肵�Ă����܂��B</para>
		/// </remarks>
		public const int MimeNoDecode = 0x2000;
		/// <summary>
		/// MIME �p�[�g�擾�Ńw�b�_�̂ݎ擾���܂��B
		/// </summary>
		private const int MimeHeader = 0x4000;
		/// <summary>
		/// ���[���ԍ��ł͂Ȃ� UID ���g�p���܂��B
		/// </summary>
		public const int UseUidValue = 0x8000;
		/// <summary>
		/// ���[���{�b�N�X�t���O�@�I���ł��Ȃ����[���{�b�N�X�ł��B
		/// </summary>
		public const int MailBoxNoSelect = 1;
		/// <summary>
		/// ���[���{�b�N�X�t���O�@�q���[���{�b�N�X���쐬�ł��܂���B
		/// </summary>
		public const int MailBoxNoInferious = 2;
		/// <summary>
		/// ���[���{�b�N�X�t���O�@���b�Z�[�W���ǉ�����Ă���\��������܂��B
		/// </summary>
		public const int MailBoxMarked = 4;
		/// <summary>
		/// ���[���{�b�N�X�t���O�@���b�Z�[�W���ǉ�����Ă��܂���B
		/// </summary>
		public const int MailBoxUnMarked = 8;
		/// <summary>
		/// ���[���{�b�N�X�t���O�@�q���[���{�b�N�X�����݂��܂��B
		/// </summary>
		public const int MailBoxChildren = 16;
		/// <summary>
		/// ���[���{�b�N�X�t���O�@�q���[���{�b�N�X�����݂��܂���B
		/// </summary>
		public const int MailBoxNoChildren = 32;
		/// <summary>
		/// ���[�����b�Z�[�W�t���O�@�ԐM�ς݃t���O�t���ł��B
		/// </summary>
		public const int MessageAnswerd = 1;
		/// <summary>
		/// ���[�����b�Z�[�W�t���O�@�����}�[�N�t���ł��B
		/// </summary>
		public const int MessageDeleted = 2;
		/// <summary>
		/// ���[�����b�Z�[�W�t���O�@���e�t���O�t���ł��B
		/// </summary>
		public const int MessageDraft = 4;
		/// <summary>
		/// ���[�����b�Z�[�W�t���O�@�d�v���t���O�t���ł��B
		/// </summary>
		public const int MessageFlagged = 8;
		/// <summary>
		/// ���[�����b�Z�[�W�t���O�@�����t���O�t���ł��B
		/// </summary>
		public const int MessageRecent = 16;
		/// <summary>
		/// ���[�����b�Z�[�W�t���O�@���ǃt���O�t���ł��B
		/// </summary>
		public const int MessageSeen = 32;
		/// <summary>
		/// ���[�������@�ԐM�ς�
		/// </summary>
		public const int SearchAnswered = 1;
		/// <summary>
		/// ���[�������@�ԐM�ς݂łȂ�
		/// </summary>
		public const int SearchUnAnswered = 2;
		/// <summary>
		/// ���[�������@�����}�[�N�t��
		/// </summary>
		public const int SearchDeleted = 3;
		/// <summary>
		/// ���[�������@�����}�[�N�Ȃ�
		/// </summary>
		public const int SearchUnDeleted = 4;
		/// <summary>
		/// ���[�������@���e�t���O����
		/// </summary>
		public const int SearchDraft = 5;
		/// <summary>
		/// ���[�������@���e�t���O�Ȃ�
		/// </summary>
		public const int SearchUnDraft = 6;
		/// <summary>
		/// ���[�������@�d�v���t���O����
		/// </summary>
		public const int SearchFlagged = 7;
		/// <summary>
		/// ���[�������@�d�v���t���O�Ȃ�
		/// </summary>
		public const int SearchUnFlagged = 8;
		/// <summary>
		/// ���[�������@�����t���O����
		/// </summary>
		public const int SearchRecent = 9;
		/// <summary>
		/// ���[�������@�����t���O�Ȃ�
		/// </summary>
		public const int SearchUnRecent = 10;
		/// <summary>
		/// ���[�������@���ǃt���O����
		/// </summary>
		public const int SearchSeen = 11;
		/// <summary>
		/// ���[�������@���ǃt���O�Ȃ�
		/// </summary>
		public const int SearchUnSeen = 12;
		/// <summary>
		/// ���[�������@�����t���O���肩���ǃt���O�Ȃ�
		/// </summary>
		public const int SearchNew = 13;
		/// <summary>
		/// ���[�������@�S�Ẵ��[��
		/// </summary>
		public const int SearchAll = 14;
		/// <summary>
		/// ���[�������@�w�肳�ꂽ�L�[���[�h����
		/// </summary>
		public const int SearchKeyword = 15;
		/// <summary>
		/// ���[�������@�w�肳�ꂽ�L�[���[�h�Ȃ�
		/// </summary>
		public const int SearchUnKeyword = 16;
		/// <summary>
		/// ���[�������@From �w�b�_�Ɏw��̕����񂠂�
		/// </summary>
		public const int SearchFrom = 17;
		/// <summary>
		/// ���[�������@To �w�b�_�Ɏw��̕����񂠂�
		/// </summary>
		public const int SearchTo = 18;
		/// <summary>
		/// ���[�������@CC �w�b�_�Ɏw��̕����񂠂�
		/// </summary>
		public const int SearchCc = 19;
		/// <summary>
		/// ���[�������@BCC �w�b�_�Ɏw��̕����񂠂�
		/// </summary>
		public const int SearchBcc = 20;
		/// <summary>
		/// ���[�������@SUBJECT �w�b�_�Ɏw��̕����񂠂�
		/// </summary>
		public const int SearchSubject = 21;
		/// <summary>
		/// ���[�������@DATE �w�b�_���w�肳�ꂽ�N�����ȑO
		/// </summary>
		public const int SearchSentBefore = 22;
		/// <summary>
		/// ���[�������@DATE �w�b�_���w�肳�ꂽ�N����
		/// </summary>
		public const int SearchSentOn = 23;
		/// <summary>
		/// ���[�������@DATE �w�b�_���w�肳�ꂽ�N�����ȍ~
		/// </summary>
		public const int SearchSentSince = 24;
		/// <summary>
		/// ���[�������@�w�肳�ꂽ�N�����ȑO�ɓ���
		/// </summary>
		public const int SearchBefore = 25;
		/// <summary>
		/// ���[�������@�w�肳�ꂽ�N�����ɓ���
		/// </summary>
		public const int SearchOn = 26;
		/// <summary>
		/// ���[�������@�w�肳�ꂽ�N�����ȍ~�ɓ���
		/// </summary>
		public const int SearchSince = 27;
		/// <summary>
		/// ���[�������@�w�肳�ꂽ�w�b�_�t�B�[���h�ɕ�������܂� �����������"�w�b�_�t�B�[���h ������"�Ǝw�肵�Ă�������
		/// </summary>
		public const int SearchHeader = 28;
		/// <summary>
		/// ���[�������@�{���ɕ�������܂�
		/// </summary>
		public const int SearchBody = 29;
		/// <summary>
		/// ���[�������@�w�b�_����і{���ɕ�������܂�
		/// </summary>
		public const int SearchText = 30;
		/// <summary>
		/// ���[�������@�w��T�C�Y���傫�ȃ��[��
		/// </summary>
		public const int SearchLager = 31;
		/// <summary>
		/// ���[�������@�w��T�C�Y��菬���ȃ��[��
		/// </summary>
		public const int SearchSmaller = 32;
		/// <summary>
		/// ���[�������@�w�� UID �̃��[��
		/// </summary>
		public const int SearchUid = 33;
		/// <summary>
		/// ���[�������@����������𒼐ڎw��
		/// </summary>
		public const int SearchCommand = 0x0100;
		/// <summary>
		/// ���[�������@�������� NOT
		/// </summary>
		public const int SearchNot = 0x0200;
		/// <summary>
		/// ���[�������@�������� OR
		/// </summary>
		public const int SearchOr = 0x0400;
		/// <summary>
		/// ���[�������@�������� AND
		/// </summary>
		public const int SearchAnd = 0x0800;
		/// <summary>
		/// ���[�������@�����������[�N������
		/// </summary>
		public const int SearchFirst = 0x1000;
		/// <summary>
		/// ���[�������@�����R�}���h���s�シ���ɖ߂�
		/// </summary>
		public const int SearchNoWait = 0x2000;
		/// <summary>
		/// ���[�������@�������ʃ`�F�b�N
		/// </summary>
		public const int SearchCheck = 0x4000;
		/// <summary>
		/// �l���O���
		/// </summary>
		private const int NameSpacePersonal = 0;
		/// <summary>
		/// ���l���O���
		/// </summary>
		private const int NameSpaceOther = 1;
		/// <summary>
		/// ���L���O���
		/// </summary>
		private const int NameSpaceShared = 2;
		/// <summary>
		/// ���O��Ԑ�
		/// </summary>
		private const int NameSpaceMax = 3;
		/// <summary>
		/// �l���O��� ���擾
		/// </summary>
		private const int NameSpacePersonalCount = 0x1000;
		/// <summary>
		/// ���l���O��� ���擾
		/// </summary>
		private const int NameSpaceOtherCount = 0x2000;
		/// <summary>
		/// ���L���O��� ���擾
		/// </summary>
		private const int NameSpaceSharedCount = 0x3000;
		/// <summary>
		/// �l���O��� �ʎ擾
		/// </summary>
		private const int NameSpacePersonalNo = 0x0100;
		/// <summary>
		/// ���l���O��� �ʎ擾
		/// </summary>
		private const int NameSpaceOtherNo = 0x0200;
		/// <summary>
		/// ���L���O��� �ʎ擾
		/// </summary>
		private const int NameSpaceSharedNo = 0x0300;
		/// <summary>
		/// ���[�����b�Z�[�W�t���O��ǉ����܂��B
		/// </summary>
		private const int AddMessage = 1;
		/// <summary>
		/// ���[�����b�Z�[�W�t���O���폜���܂��B
		/// </summary>
		private const int DeleteMessage = 2;
		/// <summary>
		/// ���[�����b�Z�[�W�t���O��u�������܂��B
		/// </summary>
		private const int ReplaceMessage = 3;
		/// <summary>
		/// ���[���{�b�N�X�̑����b�Z�[�W�����擾���܂��B
		/// </summary>
		private const int MailBoxMessage = 1;
		/// <summary>
		/// ���[���{�b�N�X�̐V�����b�Z�[�W�����擾���܂��B
		/// </summary>
		private const int MailBoxRecent = 2;
		/// <summary>
		/// ���[���{�b�N�X�̐V�������b�Z�[�W�� UID ���擾���܂��B
		/// </summary>
		private const int MailBoxNextUid = 3;
		/// <summary>
		/// ���[���{�b�N�X�� UID Validity �l���擾���܂��B
		/// </summary>
		private const int MailBoxUidValidity = 4;
		/// <summary>
		/// ���[���{�b�N�X�̖��ǃ��b�Z�[�W�����擾���܂��B
		/// </summary>
		private const int MailBoxUnSeen = 5;
		/// <summary>
		/// �{�f�B�\�� �ԍ�������
		/// </summary>
		private const int ContentPart = 0;
		/// <summary>
		/// �{�f�B�\�� Content-Type �^�C�v
		/// </summary>
		private const int ContentType = 1;
		/// <summary>
		/// �{�f�B�\�� Content-Type �T�u�^�C�v
		/// </summary>
		private const int ContentSubType = 2;
		/// <summary>
		/// �{�f�B�\�� Content-Id
		/// </summary>
		private const int ContentId = 3;
		/// <summary>
		/// �{�f�B�\�� Content-Description
		/// </summary>
		private const int ContentDescription = 4;
		/// <summary>
		/// �{�f�B�\�� Content-Transfer-Encoding
		/// </summary>
		private const int ContentTransferEncoding = 5;
		/// <summary>
		/// �{�f�B�\�� �t�@�C����
		/// </summary>
		private const int ContentFileName = 6;
		/// <summary>
		/// �{�f�B�\�� Content-Type �^�C�v���T�u�^�C�v
		/// </summary>
		private const int ContentTypeAndSubType = 7;
		/// <summary>
		/// �{�f�B�\�� �{�f�B�T�C�Y
		/// </summary>
		private const int ContentSize = 8;
		/// <summary>
		/// �{�f�B�\�� �{�f�B�s��
		/// </summary>
		private const int ContentLine = 9;
		/// <summary>
		/// �{�f�B�\�� Content-Type �p�����[�^��
		/// </summary>
		private const int ContentParameterCount = 10;
		/// <summary>
		/// �{�f�B�\�� Content-Type �p�����[�^
		/// </summary>
		private const int ContentParameter = 11;
		/// <summary>
		/// SMTP ����������
		/// </summary>
		private const int MakeDateSmtp = 0x00000;
		/// <summary>
		/// IMAP4 ����������
		/// </summary>
		private const int MakeDateImap4 = 0x10000;
		/// <summary>
		/// ���ݓ���
		/// </summary>
		private const int MakeDateNow = 0x20000;
		/// <summary>
		/// �^�C���]�[���w�� �{
		/// </summary>
		private const int MakeDateTimeZonePlus = 0x40000;
		/// <summary>
		/// �^�C���]�[���w�� �[
		/// </summary>
		private const int MakeDateTimeZoneMinus = 0x80000;
		/// <summary>
		/// SMTP����������(RFC2822)���擾���܂��B��: "Fri, 9 Jul 2009 15:10:30 +0900
		/// </summary>
		private const int DateTimeSmtp = 0x0000;
		/// <summary>
		/// IMAP4���t������(RFC2060)���擾���܂��B�����p ��: "09-Jul-2009"
		/// </summary>
		private const int DateImap4 = 0x8000;
		/// <summary>
		/// IMAP4����������(RFC2060)���擾���܂��BAppend�p ��: "09-Jul-2009 15:10:30"
		/// </summary>
		private const int DateTimeImap4 = 0x8000;
		/// <summary>
		/// ����������̃o�b�t�@�T�C�Y�ł��B
		/// </summary>
		private const int DateStringSize = 34;
		/// <summary>
		/// SSLv3 ���g�p���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int SSL3 = 0x1000;
		/// <summary>
		/// TSLv1 ���g�p���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int TLS1 = 0x2000;
		/// <summary>
		/// STARTTLS ���g�p���܂��B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int STARTTLS = 0x4000;
		/// <summary>
		/// �T�[�o�ؖ����������؂�ł��G���[�ɂ��܂���B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int IgnoreNotTimeValid = 0x0800;
		/// <summary>
		/// ���[�g�ؖ����������ł��G���[�ɂ��܂���B
		/// </summary>
		/// <remarks>
		/// <see cref="SSL"/>�Ŏw�肵�܂��B
		/// </remarks>
		public const int AllowUnknownCA = 0x0400;
		/// <summary>
		/// IMAP4 �̕W���|�[�g�ԍ��ł�
		/// </summary>
		public const int StandardPortNo = 143;
		/// <summary>
		/// IMAP4 over SSL �̃|�[�g�ԍ��ł�
		/// </summary>
		public const int StandardSslPortNo = 993;

		/// <summary>
		/// IMAP4 �|�[�g�ԍ��ł��B
		/// </summary>
		protected int _port = 143;
		/// <summary>
		/// �\�P�b�g�n���h���ł��B
		/// </summary>
		protected IntPtr _socket = (IntPtr)ErrorSocket;
		/// <summary>
		/// ���[�����ł��B
		/// </summary>
		protected int _count = -1;
		/// <summary>
		/// ���[���T�C�Y�ł��B
		/// </summary>
		protected int _size = -1;
		/// <summary>
		/// �w�b�_�[�T�C�Y�ł��B
		/// </summary>
		protected int _header_size = -1;
		/// <summary>
		/// �{���̃T�C�Y�ł��B
		/// </summary>
		protected int _body_size = -1;
		/// <summary>
		/// ���[����M���̐ݒ�p�t���O�ł��B
		/// </summary>
		protected int _flag = 0;
		/// <summary>
		/// ���[�����b�Z�[�W�̃t���O�ł��B
		/// </summary>
		protected int _message_flag = 0;
		/// <summary>
		/// ���[�����w�肷��ۂ� UID ���g�p���܂��B
		/// </summary>
		protected bool _useuid_flag = false;
		/// <summary>
		/// MIME �p�[�g�擾�������͕ۑ��̍ۂ̐ݒ�t���O�ł��B
		/// </summary>
		protected int _mime_flag = 0;
		/// <summary>
		/// ���[�����w�肷��ۂ� UID ���g�p���܂��B
		/// </summary>
		protected int _useuid = 0;
		/// <summary>
		/// �G���[�ԍ��ł��B
		/// </summary>
		protected int _err = 0;
		/// <summary>
		/// IMAP4 �F�،`��
		/// </summary>
		protected int _mode = AuthLogin;
		/// <summary>
		/// IMAP4 �T�[�o���ł��B
		/// </summary>
		protected string _host = "";
		/// <summary>
		/// IMAP4 ���[�U�[���ł��B
		/// </summary>
		protected string _id = "";
		/// <summary>
		/// IMAP4 �p�X���[�h�ł��B
		/// </summary>
		protected string _password = "";
		/// <summary>
		/// ���[���{�b�N�X���ł��B
		/// </summary>
		protected string _mailbox = "";
		/// <summary>
		/// �Y�t�t�@�C���ۑ��p�̃p�X�ł��B
		/// </summary>
		protected string _path = null;
		/// <summary>
		/// �w�b�_�t�B�[���h���ł��B
		/// </summary>
		protected string _field_name = "";
		/// <summary>
		/// �{���i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _body = null;
		/// <summary>
		/// �����i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _subject = null;
		/// <summary>
		/// ���t������ۑ��o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _date = null;
		/// <summary>
		/// ���o�l�i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _from = null;
		/// <summary>
		/// �w�b�_�i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _header = null;
		/// <summary>
		/// �Y�t�t�@�C�����i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _filename = null;
		/// <summary>
		/// �Y�t�t�@�C�����̃��X�g�ł��B
		/// </summary>
		protected string[] _filename_list = null;
		/// <summary>
		/// �w�b�_�t�B�[���h���e�i�[�o�b�t�@�ł��B
		/// </summary>
		protected StringBuilder _field = null;
		/// <summary>
		/// UID �ł��B
		/// </summary>
		protected uint _uid = 0;
		/// <summary>
		/// Dispose �������s�������ǂ����̃t���O�ł��B
		/// </summary>
		private bool _disposed = false;
		/// <summary>
		/// text/html �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		protected StringBuilder _html_file = null;
		/// <summary>
		/// SSL �ݒ�t���O�ł��B
		/// </summary>
		protected int _ssl = 0;
		/// <summary>
		/// SSL �N���C�A���g�ؖ������ł��B
		/// </summary>
		protected string _cert_name = null;
		/// <summary>
		/// message/rfc822 �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		protected StringBuilder _rfc822_file = null;

		/// <summary>
		/// ���[���{�b�N�X���ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="GetMailBoxList"/>��������<see cref="GetMailBoxListSubscribe"/>�̌��ʂ��i�[�����\���̂ł��B
		/// </remarks>
		public struct MailBoxStatus
		{
			/// <summary>
			/// ���[���{�b�N�X�t���O�ł��B
			/// </summary>
			/// <remarks>
			/// <para><see cref="MailBoxNoSelect"/>�I���ł��Ȃ����[���{�b�N�X�ł�</para>
			/// <para><see cref="MailBoxNoInferious"/>�q���[���{�b�N�X���쐬�ł��܂���</para>
			/// <para><see cref="MailBoxMarked"/>���b�Z�[�W���ǉ�����Ă���\��������܂�</para>
			/// <para><see cref="MailBoxUnMarked"/>���b�Z�[�W���ǉ�����Ă��܂���</para>
			/// <para><see cref="MailBoxChildren"/>�q���[���{�b�N�X�����݂��܂�</para>
			/// <para><see cref="MailBoxNoChildren"/>�q���[���{�b�N�X�����݂��܂���</para>
			/// </remarks>
			public int Flag;
			/// <summary>
			/// ���[���{�b�N�X���ł��B
			/// </summary>
			public string Name;
			/// <summary>
			/// ���[���{�b�N�X��؂蕶���ł��B
			/// </summary>
			public char Separate;
		}
		/// <summary>
		/// ���[���{�b�N�X��񃊃X�g�ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="GetMailBoxList"/>��������<see cref="GetMailBoxListSubscribe"/>���s��Ɍ��ʂ��i�[����܂��B
		/// </remarks>
		protected MailBoxStatus[] _mailbox_list = null;
		/// <summary>
		/// MIME �\�� Content-Type �t�B�[���h�̃p�����[�^���ł��B
		/// </summary>
		/// <remarks>
		/// MIME �\�� Content-Type �t�B�[���h�̃p�����[�^���ł��B
		/// MIME �\����� <see cref="MimeStructureStatus"/> �Ɋ܂܂�܂��B
		/// </remarks>
		public struct MimeParameterStatus
		{
			/// <summary>
			/// �p�����[�^���̂ł��B
			/// </summary>
			public string Name;
			/// <summary>
			/// �p�����[�^�̒l�ł��B
			/// </summary>
			public string Value ;
		}
		/// <summary>
		/// MIME �\�����ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="GetMimeStructure"/>�̌��ʂ��i�[�����\���̂ł��B
		/// </remarks>
		public struct MimeStructureStatus
		{
			/// <summary>
			/// MIME �p�[�g�ԍ��ł��B
			/// </summary>
			/// <remarks>
			/// <see cref="GetMimePart"/>��<see cref="SaveMimePart"/>�� part_no �Ŏw�肷��l�ł��B
			/// </remarks>
			public int PartNo;
			/// <summary>
			/// MIME �\����\��������ł��B
			/// </summary>
			public string Part;
			/// <summary>
			/// Content-Type �t�B�[���h�̃^�C�v�ł��B
			/// </summary>
			public string Type;
			/// <summary>
			/// Content-Type �t�B�[���h�̃T�u�^�C�v�ł��B
			/// </summary>
			public string SubType;
			/// <summary>
			/// Contetn-ID �t�B�[���h�̒l�ł��B
			/// </summary>
			public string Id;
			/// <summary>
			/// Content-Description �t�B�[���h�̒l�ł��B
			/// </summary>
			public string Description;
			/// <summary>
			/// Content-Transfer-Encoding �t�B�[���h�̒l�ł��B
			/// </summary>
			public string Encoding;
			/// <summary>
			/// �Y�t�t�@�C�����ł��B(�f�R�[�h�ς�)
			/// </summary>
			public string FileName;
			/// <summary>
			/// MIME �p�[�g�̃T�C�Y�ł��B
			/// </summary>
			public int Size;
			/// <summary>
			/// MIME �p�[�g�̍s���ł��B�^�C�v�� text �̏ꍇ�̂ݐ������l������܂��B
			/// </summary>
			public int Line;
			/// <summary>
			/// Content-Type �̃p�����[�^���X�g�ł��B
			/// </summary>
			public MimeParameterStatus[] Parameter;
		}
		/// <summary>
		/// MIME �\����񃊃X�g�ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="GetMimeStructure"/>���s��Ɍ��ʂ��i�[����܂��B
		/// </remarks>
		protected MimeStructureStatus[] _mime_list = null;
		/// <summary>
		/// �������ʂ̃��X�g�ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="Search"/>���s��Ɍ��ʂ��i�[����܂��B
		/// </remarks>
		protected uint[] _search_list = null;
		/// <summary>
		/// �ŏ��̌������ǂ����̃t���O�ł��B
		/// </summary>
		protected bool _search_first_flag = true;
		/// <summary>
		/// ���O��ԏ��ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="GetNameSpace"/>�̌��ʂ��i�[�����\���̂ł��B
		/// </remarks>
		public struct NameSpaceStatus
		{
			/// <summary>
			/// ���O��Ԃ̃v���t�B�b�N�X�ł��B
			/// </summary>
			public string Name;
			/// <summary>
			/// ���O��Ԃ̋�؂蕶���ł��B
			/// </summary>
			public char Separate;
		}
		/// <summary>
		/// ���O��ԏ�񃊃X�g�ł��B
		/// </summary>
		/// <remarks>
		/// <see cref="GetNameSpace"/>���s��Ɍ��ʂ��i�[����܂��B
		/// </remarks>
		protected NameSpaceStatus[][] _namespace_list = new NameSpaceStatus[NameSpaceMax][];

		/// <summary>
		/// <c>Imap4</c>�N���X�̐V�K�C���X�^���X�����������܂��B
		/// </summary>
		public Imap4()
		{
			Init();
		}
		/// <summary>
		/// <c>Imap4</c>�N���X�̐V�K�C���X�^���X�����������܂��B
		/// </summary>
		/// <param name="host_name">IMAP4 �T�[�o�[��</param>
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
		/// <see cref="Imap4"/>�ɂ���Ďg�p����Ă��邷�ׂẴ��\�[�X��������܂��B
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// <see cref="Imap4"/>�ɂ���Ďg�p����Ă��邷�ׂẴ��\�[�X��������܂��B
		/// </summary>
		/// <param name="disposing">
		/// �}�l�[�W���\�[�X�ƃA���}�l�[�W���\�[�X�̗������������ꍇ��<c>true</c>�B
		/// �A���}�l�[�W���\�[�X�������������ꍇ��<c>false</c>�B
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
		/// �����������ł��B
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
		/// �w�b�_�i�[�p�o�b�t�@�̃T�C�Y�����肵�܂��B
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
		/// �T�[�o�ɐڑ��������肷��
		/// </summary>
		/// <returns>true �ŃT�[�o�ɐڑ���</returns>
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
		/// IMAP4 �T�[�o�ɐڑ����܂��B
		/// </summary>
		/// <remarks>
		/// IMAP4 �T�[�o�ɐڑ����܂��B
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	IMAP4 �T�[�o�[�Ƃ̐ڑ��Ɏ��s���܂����B
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// IMAP4 �T�[�o�ɐڑ����܂��B
		/// </summary>
		/// <param name="host_name">IMAP4 �T�[�o�[��</param>
		/// <remarks>
		/// �T�[�o�����w�肵�� IMAP4 �T�[�o�ɐڑ����܂��B
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	IMAP4 �T�[�o�Ƃ̐ڑ��Ɏ��s���܂����B
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Connect(string host_name)
		{
			_host = host_name;
			Connect();
		}
		/// <summary>
		/// IMAP4 �T�[�o�ɐڑ����܂��B
		/// </summary>
		/// <param name="host_name">IMAP4 �T�[�o��</param>
		/// <param name="port_no">�|�[�g�ԍ�</param>
		/// <remarks>
		/// �T�[�o���ƃ|�[�g�ԍ����w�肵�� IMAP4 �T�[�o�ɐڑ����܂��B
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	IMAP4 �T�[�o�Ƃ̐ڑ��Ɏ��s���܂����B
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Connect(string host_name, int port_no)
		{
			_host = host_name;
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// IMAP4 �T�[�o�ɐڑ����܂��B
		/// </summary>
		/// <param name="port_no">�|�[�g�ԍ�</param>
		/// <remarks>
		/// �|�[�g�ԍ����w�肵�� IMAP4 �T�[�o�ɐڑ����܂��B
		/// </remarks>
		///	<exception cref="ArgumentOutOfRangeException">
		///	�|�[�g�ԍ�������������܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	IMAP4 �T�[�o�Ƃ̐ڑ��Ɏ��s���܂����B
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Connect(int port_no)
		{
			_port = port_no;
			Connect();
		}
		/// <summary>
		/// IMAP4 �T�[�o�Ƃ̐ڑ����I�����܂��B
		/// </summary>
		public void Close()
		{
			if(_socket != (IntPtr)ErrorSocket) {
				Imap4Close(_socket);
			}
			_socket = (IntPtr)ErrorSocket;
		}
		/// <summary>
		/// IMAP4 �T�[�o�F�؂��s���܂��B
		/// </summary>
		/// <remarks>
		/// ID �ƃp�X���[�h���w�肵�� IMAP4 �T�[�o�F�؂��s���܂��B
		/// </remarks>
		/// <param name="id_str">IMAP4 ���[�U�[ ID</param>
		/// <param name="pass_str">IMAP4 �p�X���[�h</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="FormatException">
		///	ID �������̓p�X���[�h�ɕ����񂪓����Ă��܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// IMAP4 �T�[�o�F�؂��s���܂��B
		/// </summary>
		/// <remarks>
		/// ID�A�p�X���[�h����єF�،`�����w�肵�� IMAP4 �T�[�o�F�؂��s���܂��B
		/// </remarks>
		/// <param name="id_str">IMAP4 ���[�U�[ ID</param>
		/// <param name="pass_str">IMAP4 �p�X���[�h</param>
		/// <param name="mode">�F�،`��
		/// <para>�F�،`���̐ݒ�\�Ȓl�͉��L�̒ʂ�ł��B</para>
		/// <para><see cref="AuthLogin"/> �� LOGIN ���g�p���܂��B</para>
		/// <para><see cref="AuthPlain"/> �� PLAIN ���g�p���܂��B</para>
		/// <para><see cref="AuthCramMd5"/> CRAM MD5 ���g�p���܂��B</para>
		/// <para><see cref="AuthDigestMd5"/> DIGEST MD5 ���g�p���܂��B</para>
		/// <para>C# �� | �AVB.NET �ł� Or �ŕ����w��\�ł��B</para>
		/// <para>DIGEST MD5��CRAM MD5��PLAIN��LOGIN �̗D�揇�ʂŔF�؂����݂܂��B</para>
		/// </param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="FormatException">
		///	ID �������̓p�X���[�h�ɕ����񂪓����Ă��܂���B
		/// </exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Authenticate(string id_str, string pass_str, int mode)
		{
			_mode = mode;
			Authenticate(id_str, pass_str);
		}

		/// <summary>
		/// �w�Ǎς݃��[���{�b�N�X�̈ꗗ���擾���܂��B
		/// </summary>
		/// <param name="refer">�K�w�ʒu�ł��B</param>
		/// <param name="mailbox">���C���h�J�[�h�ł��B"*" �őS�āA"%" �œ����K�w�̃��[���{�b�N�X���������܂��B</param>
		/// <remarks>
		/// �w�Ǎς݃��[���{�b�N�X�̈ꗗ���擾���܂��B�ꗗ��<see cref="GetMailBoxStatusList"/> �Ŏ擾�ł��܂��B�B
		/// <example>
		/// <para>
		///	�S�Ă̍w�Ǎς݃��[���{�b�N�X�̈ꗗ��\�����܂��B
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			// �w�Ǎς݃��[���{�b�N�X�ꗗ���擾
		///			imap.GetMailBoxListSubscribe("", "*");
		///			nMail.Imap4.MailBoxStatus [] list = imap4.GetMailBoxStatusList()
		///			foreach(nMail.Imap4.MailBoxStatus mailbox in list)
		///			{
		///				MessageBox.Show(string.Format("���[���{�b�N�X��:{0:s} ��؂蕶��:{1:c} �t���O:{2:d}", mailbox.Name, mailbox.Separate, mailbox.Flag));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(string.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(string.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		' �w�Ǎς݃��[���{�b�N�X�ꗗ���擾
		///		imap.GetMailBoxListSubscribe("", "*")
		///		Dim list As nMail.Imap4.MailBoxStatus() = imap4.GetMailBoxList()
		///		For Each mailbox As nMail.Imap4.MailBoxStatus In list
		///			MessageBox.Show(String.Format("���[���{�b�N�X��:{0:s} ��؂蕶��:{1:c} �t���O:{2:d}", mailbox.Name, mailbox.Separate, mailbox.Flag))
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���{�b�N�X�̈ꗗ���擾���܂��B
		/// </summary>
		/// <remarks>
		/// ���[���{�b�N�X�̈ꗗ���擾���܂��B�ꗗ��<see cref="GetMailBoxList"/> �Ŏ擾�ł��܂��B
		/// <example>
		/// <para>
		///	�S�Ẵ��[���{�b�N�X�̈ꗗ��\�����܂��B
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			// ���[���{�b�N�X�ꗗ���擾
		///			imap.GetMailBoxList("", "*");
		///			nMail.Imap4.MailBoxStatus [] list = imap4.GetMailBoxStatusList()
		///			foreach(nMail.Imap4.MailBoxStatus mailbox in list)
		///			{
		///				MessageBox.Show(string.Format("���[���{�b�N�X��:{0:s} ��؂蕶��:{1:c} �t���O:{2:d}", mailbox.Name, mailbox.Separate, mailbox.Flag));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(string.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(string.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		' ���[���{�b�N�X�ꗗ���擾
		///		imap.GetMailBoxList("", "*")
		///		Dim list As nMail.Imap4.MailBoxStatus() = imap4.GetMailBoxList()
		///		For Each mailbox As nMail.Imap4.MailBoxStatus In list
		///			MessageBox.Show(String.Format("���[���{�b�N�X��:{0:s} ��؂蕶��:{1:c} �t���O:{2:d}", mailbox.Name, mailbox.Separate, mailbox.Flag))
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <param name="refer">�K�w�ʒu�ł��B</param>
		/// <param name="mailbox">���C���h�J�[�h�ł��B"*" �őS�āA"%" �œ����K�w�̃��[���{�b�N�X���������܂��B</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���{�b�N�X�̃��b�Z�[�W�������擾���܂��B
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>�p�����[�^�Ń��b�Z�[�W�������擾���������[���{�b�N�X�����w�肵�܂��B
		/// </remarks>
		/// <param name="mailbox">���b�Z�[�W�������擾���������[���{�b�N�X���ł��B</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		/// <returns>���b�Z�[�W����</returns>
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
		/// ���[���{�b�N�X�̐V�����b�Z�[�W�����擾���܂��B
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>�p�����[�^�ŐV�����b�Z�[�W�����擾���������[���{�b�N�X�����w�肵�܂��B
		/// </remarks>
		/// <param name="mailbox">�V�����b�Z�[�W�����擾���������[���{�b�N�X���ł��B</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		/// <returns>�V�����b�Z�[�W��</returns>
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
		/// ���[���{�b�N�X�̐V�������b�Z�[�W�� UID ���擾���܂��B
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>�p�����[�^�ŐV�������b�Z�[�W�� UID �l���擾���������[���{�b�N�X�����w�肵�܂��B
		/// </remarks>
		/// <param name="mailbox">�V�������b�Z�[�W�� UID �l���擾���������[���{�b�N�X���ł��B</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		/// <returns>�V�������b�Z�[�W�� UID</returns>
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
		/// ���[���{�b�N�X�� UID Validity �l���擾���܂��B
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>�p�����[�^�� UID Validity �l���擾���������[���{�b�N�X�����w�肵�܂��B
		/// </remarks>
		/// <param name="mailbox">UID Validity �l���擾���������[���{�b�N�X���ł��B</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		/// <returns>���[���{�b�N�X�� UID Validity</returns>
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
		/// ���[���{�b�N�X�̖��ǃ��b�Z�[�W�����擾���܂��B
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>�p�����[�^�Ŗ��ǃ��b�Z�[�W�����擾���������[���{�b�N�X�����w�肵�܂��B
		/// </remarks>
		/// <param name="mailbox">���ǃ��b�Z�[�W�����擾���������[���{�b�N�X���ł��B</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		/// <returns>���ǃ��b�Z�[�W��</returns>
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
		/// ���[���{�b�N�X��I�����܂��B
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>�p�����[�^�őI�����������[���{�b�N�X�����w�肵�܂��B
		/// </remarks>
		/// <param name="mailbox">�I�����郁�[���{�b�N�X���ł��B</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���{�b�N�X��ǂݏo����p�őI�����܂��B
		/// </summary>
		/// <remarks>
		/// <paramref name="mailbox"/>�p�����[�^�œǂݏo����p�őI�����������[���{�b�N�X�����w�肵�܂��B
		/// </remarks>
		/// <param name="mailbox">�I�����郁�[���{�b�N�X���ł��B</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���̃X�e�[�^�X���擾���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <remarks>
		/// <paramref name="no"/>�p�����[�^�ŃX�e�[�^�X���擾���������[���ԍ����w�肵�܂��B
		/// <para>������<see cref="Subject"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���t�������<see cref="DateString"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���o�l��<see cref="From"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�w�b�_��<see cref="Header"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���b�Z�[�W�t���O��<see cref="MessageFlag"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�X�e�[�^�X�擾���s�̏ꍇ�̃G���[�ԍ���<see cref="nMailException.ErrorCode"/>�Ŏ擾�ł��܂��B</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���̃T�C�Y���擾���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <remarks>
		/// <paramref name="no"/>�p�����[�^�Ń��[���T�C�Y���擾���������[���ԍ����w�肵�܂��B
		/// <example>
		/// <para>
		/// ���[���{�b�N�X("inbox")���̃��[���ԍ�(�ϐ���:no)�̃��[���T�C�Y���擾����B
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.GetSize(no);
		///			MessageBox.Show(String.Format("���[���ԍ�:{0:d},�T�C�Y:{1:d}", no, imap.Size));
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.GetMail(no)
		///		MessageBox.Show(String.Format("���[���ԍ�:{0:d},�T�C�Y:{1:d}", no, imap.Size))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		/// <returns>���[���T�C�Y</returns>
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
		/// ���[�����擾���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <remarks>
		/// <paramref name="no"/>�p�����[�^�Ń��[�����擾���������[���ԍ����w�肵�܂��B
		/// <para>�Y�t�t�@�C����ۑ��������ꍇ�A<see cref="Path"/>�ɕۑ��������t�H���_���w�肵�Ă����܂��B</para>
		/// <para>�g���@�\���g�p�������ꍇ�A<see cref="Flag"/>�Őݒ肵�Ă����܂��B</para>
		/// <para>������<see cref="Subject"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���t�������<see cref="DateString"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���o�l��<see cref="From"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�w�b�_��<see cref="Header"/>�Ŏ擾�ł��܂��B</para>
		/// <para>���[���T�C�Y��<see cref="Size"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�Y�t�t�@�C������<see cref="FileName"/>�Ŏ擾�ł��܂��B</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="DirectoryNotFoundException">
		///	<see cref="Path"/>�Ŏw�肵���t�H���_�����݂��܂���B
		/// </exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���̍폜�}�[�N��t�����܂��B
		///	���ۂ̍폜��<see cref="Expunge"/>���\�b�h���Ăяo�����ۂɍs���܂��B
		/// </summary>
		/// <param name="no">�폜�}�[�N��t�����郁�[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <remarks>
		/// <para>���[���폜�}�[�N�t�����s�̏ꍇ�̃G���[�ԍ���<see cref="nMailException.ErrorCode"/>�Ŏ擾�ł��܂��B</para>
		/// <example>
		/// <para>
		/// ���[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)�ɍ폜�}�[�N��t�����A���ۂɍ폜���܂��B
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.Delete(no);	// �폜�}�[�N�t��
		///			imap.Expunge();		// �폜���s
		///			MessageBox.Show(String.Format("���[���ԍ�:{0:d}���폜����", no));
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.Delete(no)			' �폜�}�[�N�t��
		///		imap.Expunge()			' �폜���s
		///		MessageBox.Show(String.Format("���[���ԍ�:{0:d}���폜����", no))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// �폜�}�[�N�̂��Ă��郁�[�����폜���܂��B
		/// </summary>
		/// <remarks>
		/// <para>�폜�}�[�N�̂��Ă��郁�[�����폜���܂��B</para>
		/// <para>���[���폜���s�̏ꍇ�̃G���[�ԍ���<see cref="nMailException.ErrorCode"/>�Ŏ擾�ł��܂��B</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���{�b�N�X�̑I�����I�����܂��B
		/// </summary>
		/// <remarks>
		/// <para>���[���{�b�N�X�̑I�����I�����܂��B</para>
		/// <para>���[���{�b�N�X�̑I���I�����s�̏ꍇ�̃G���[�ԍ���<see cref="nMailException.ErrorCode"/>�Ŏ擾�ł��܂��B</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[�����b�Z�[�W�t���O��t�����܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <param name="message_flag">���b�Z�[�W�t���O
		/// <para><see cref="MessageAnswerd"/>�ԐM�ς݃t���O</para>
		/// <para><see cref="MessageDeleted"/>�����}�[�N</para>
		/// <para><see cref="MessageDraft"/>���e�t���O</para>
		/// <para><see cref="MessageFlagged"/>�d�v���t���O</para>
		/// <para><see cref="MessageRecent"/>�����t���O</para>
		/// <para><see cref="MessageSeen"/>���ǃt���O</para>
		/// <para>C# �� | �AVB.NET �ł� Or �ŕ����w��\�ł��B</para>
		///	</param>
		/// <remarks>
		/// �w�肵�����b�Z�[�W�t���O��t�����܂��B�w�肵�Ă��Ȃ��t���O�̏�Ԃ͂��̂܂܂ł��B
		/// <para>�t���O�t�����s�̏ꍇ�̃G���[�ԍ���<see cref="nMailException.ErrorCode"/>�Ŏ擾�ł��܂��B</para>
		/// <example>
		/// <para>
		/// ���[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)�ɕԐM�ς݃t���O��t�����܂��B
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.AddMessageFlag(no, nMail.Imap4.MessageAnswerd);	// �ԐM�ς݃t���O�t��
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.AddMessageFlag(no, nMail.Imap4.MessageAnswerd)		' �ԐM�ς݃t���O�t��
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[�����b�Z�[�W�t���O���폜���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <param name="message_flag">���b�Z�[�W�t���O
		/// <para><see cref="MessageAnswerd"/>�ԐM�ς݃t���O</para>
		/// <para><see cref="MessageDeleted"/>�����}�[�N</para>
		/// <para><see cref="MessageDraft"/>���e�t���O</para>
		/// <para><see cref="MessageFlagged"/>�d�v���t���O</para>
		/// <para><see cref="MessageRecent"/>�����t���O</para>
		/// <para><see cref="MessageSeen"/>���ǃt���O</para>
		/// <para>C# �� | �AVB.NET �ł� Or �ŕ����w��\�ł��B</para>
		///	</param>
		/// <remarks>
		/// �w�肵�����b�Z�[�W�t���O���폜���܂��B�w�肵�Ă��Ȃ��t���O�̏�Ԃ͂��̂܂܂ł��B
		/// <para>���[�����b�Z�[�W�t���O�t�����s�̏ꍇ�̃G���[�ԍ���<see cref="nMailException.ErrorCode"/>�Ŏ擾�ł��܂��B</para>
		/// <example>
		/// <para>
		/// ���[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)����폜�}�[�N�t���O���폜���܂��B
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.DeleteMessageFlag(no, nMail.Imap4.MessageDeleted);	// �폜�}�[�N�t���O�폜
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.DeleteMessageFlag(no, nMail.Imap4.MessageDeleted)		' �폜�}�[�N�t���O�폜
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[�����b�Z�[�W�t���O��u�������܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <param name="message_flag">���b�Z�[�W�t���O
		/// <para><see cref="MessageAnswerd"/>�ԐM�ς݃t���O</para>
		/// <para><see cref="MessageDeleted"/>�����}�[�N</para>
		/// <para><see cref="MessageDraft"/>���e�t���O</para>
		/// <para><see cref="MessageFlagged"/>�d�v���t���O</para>
		/// <para><see cref="MessageRecent"/>�����t���O</para>
		/// <para><see cref="MessageSeen"/>���ǃt���O</para>
		/// <para>C# �� | �AVB.NET �ł� Or �ŕ����w��\�ł��B</para>
		///	</param>
		/// <remarks>
		/// �w�肵�����b�Z�[�W�t���O�Œu�������܂��B
		/// <para>���[�����b�Z�[�W�t���O�u���������s�̏ꍇ�̃G���[�ԍ���<see cref="nMailException.ErrorCode"/>�Ŏ擾�ł��܂��B</para>
		/// <example>
		/// <para>
		/// ���[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)�����ǂ���яd�v���b�Z�[�W�t���O���t���Ă����ԂƂ��܂��B
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		/// 		imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.ReplaceMessageFlag(no, nMail.Imap4.MessageFlagged | nMail.Imap4.MessageSeen);	// ���ǂ���яd�v���t���O
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.ReplaceMessageFlag(no, nMail.Imap4.MessageFlagged Or nMail.Imap4.MessageSeen)	' ���ǋy�яd�v���t���O
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// �Y�t�t�@�C���̑��݃`�F�b�N���s���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <remarks>
		///	��������Ă��郁�[���ɂ��Ă͈�x��M��AAttachment �N���X�Ō������Ă��������B
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <returns>true �œY�t�t�@�C��������</returns>
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
		/// ���[���� UID ���擾���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�</param>
		/// <remarks>
		/// <para>�擾���� UID �͕Ԓl�������� <see cref="Uid"/>�Ŏ擾�ł��܂��B</para>
		/// <example>
		/// <para>
		/// ���[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)�� UID ���擾����B
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
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		/// Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.GetUid(no)
		///		MessageBox.Show("Uid=" + imap.Uid.ToString())
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		/// <returns>���[���� UID</returns>
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
		/// ���[���w�b�_����w��̃t�B�[���h�̓��e���擾���܂��B
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <returns>�t�B�[���h�̓��e</returns>
		/// <remarks>
		/// IMAP4 �T�[�o�Ƃ̐ڑ��Ƃ͖��֌W�Ɏg�p�ł��܂��B
		/// <para>�w�b�_�́A<see cref="Header"/>�Őݒ肵�Ă����܂��B
		/// <see cref="GetMail"/>�Ŏ�M��������ɌĂяo�����ꍇ�A
		/// ��M�������[���̃w�b�_���g�p���܂��B</para>
		/// <para>�擾�����t�B�[���h���e��<see cref="Field"/>�Ŏ擾�ł��܂��B</para>
		/// <example>
		/// <para>
		/// ���[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ�:no)�� X-Mailer �w�b�_�t�B�[���h���擾����B
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
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception e)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		/// Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		/// 	imap.Connect()
		/// 	imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.GetMail(no)
		///		MessageBox.Show("X-Mailer:" + imap.GetHeaderField("X-Mailer:"))
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		/// <returns>�t�B�[���h�̓��e</returns>
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
		/// ���[���w�b�_����w��̃t�B�[���h�̓��e���擾���܂��B
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <param name="header">�w�b�_</param>
		/// <returns>�t�B�[���h�̓��e</returns>
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
		/// MIME �w�b�_�t�B�[���h�̕�������f�R�[�h���܂�
		/// </summary>
		/// <param name="field">�t�B�[���h�̕�����</param>
		/// <returns>�f�R�[�h�����t�B�[���h���e</returns>
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
		/// ���[���w�b�_����w��̃w�b�_�t�B�[���h�̓��e���擾���A
		/// MIME �w�b�_�t�B�[���h�̃f�R�[�h���s���ĕԂ��܂�
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <returns>�擾�����f�R�[�h�ς݂̃t�B�[���h���e</returns>
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
		/// ���[���w�b�_����w��̃w�b�_�t�B�[���h�̓��e���擾���A
		/// MIME �w�b�_�t�B�[���h�̃f�R�[�h���s���ĕԂ��܂�
		/// </summary>
		/// <param name="field_name">�t�B�[���h��</param>
		/// <param name="header">�w�b�_</param>
		/// <returns>�擾�����f�R�[�h�ς݂̃t�B�[���h���e</returns>
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
		/// ���[���{�b�N�X���쐬���܂��B
		/// </summary>
		/// <param name="mailbox">�쐬���郁�[���{�b�N�X��</param>
		/// <remarks>
		/// ���[���{�b�N�X���쐬���܂��B
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���{�b�N�X���폜���܂��B
		/// </summary>
		/// <remarks>
		/// ���[���{�b�N�X���폜���܂��B
		/// </remarks>
		/// <param name="mailbox">�폜���郁�[���{�b�N�X��</param>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���{�b�N�X����ύX���܂��B
		/// </summary>
		/// <param name="old_name">�ύX����O�̃��[���{�b�N�X��</param>
		/// <param name="new_name">�ύX��̃��[���{�b�N�X��</param>
		/// <remarks>
		/// ���[���{�b�N�X����ύX���܂��B
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���{�b�N�X���w�ǂ��܂��B
		/// </summary>
		/// <param name="mailbox">�w�ǂ��郁�[���{�b�N�X��</param>
		/// <remarks>
		/// ���[���{�b�N�X���w�ǂ��܂��B
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���{�b�N�X�̍w�ǂ��������܂��B
		/// </summary>
		/// <param name="mailbox">�w�ǉ������郁�[���{�b�N�X��</param>
		/// <remarks>
		/// ���[���{�b�N�X�̍w�ǂ��������܂��B
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[�����R�s�[���܂��B
		/// </summary>
		/// <param name="no">�R�s�[���郁�[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <param name="mailbox">�R�s�[��̃��[���{�b�N�X��</param>
		/// <remarks>
		/// ���ݑI������Ă��郁�[���{�b�N�X�̃��[�����w��̃��[���{�b�N�X�ɃR�s�[���܂��B
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// MIME �\�����擾���܂��B
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <remarks>
		/// <para>MIME �\����<see cref="GetMimeStructureStatusList"/>�Ŏ擾�ł��܂��B</para>
		/// <example>
		/// <para>
		///	���[���{�b�N�X("inbox")���̎w��̃��[���ԍ�(�ϐ���:no)�� MIME �\�����擾���A�e�L�X�g�ł���Ζ{����\���A�Y�t�t�@�C���ł���� z:\temp �ɕۑ����܂��B
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			// MIME �\�����擾
		///			imap.GetMimeStructure(no);
		///			nMail.Imap4.MimeStructureStatus [] list = imap4.GetMimeStructureStatusList()
		///			foreach(nMail.Imap4.MimeStructureStatus mime in list)
		///			{
		///				// �e�L�X�g�ł���Ζ{�����擾���ĕ\��
		///				if(string.Compare(mime.Type, "text", true) == 0 &amp;&amp; string.Compare(mime.SubType, "plain", true) == 0)
		///				{
		///					// �{���擾
		///					imap.GetMimePart(no, mime.PartNo);
		///					MessageBox.Show(imap.Body);
		///				}
		///				else if(string.Compare(mime.Type, "multipart", true) != 0)
		///				{
		///					// �Y�t�t�@�C����ۑ�
		///					imap.SaveMimePart(no, mime.PartNo, @"z:\temp\" + mime.FileName);
		///				}
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		' MIME �\�����擾
		///		imap.GetMimeStructure(no)
		///		Dim list As nMail.Imap4.MimeStructureStatus() = imap4.GetMimeStructureList()
		///		For Each mime As nMail.Imap4.MimeStructureStatus In list
		///			' �e�L�X�g�ł���Ζ{�����擾���ĕ\��
		///			If (String.Compare(mime.Type, "text", True) = 0) And (String.Compare(mime.SubType, "plain", True) = 0) Then
		///				' �{���擾
		///				imap.GetMimePart(no, mime.PartNo)
		///				MessageBox.Show(imap.Body)
		///			Else If String.Compare(mime.Type, "multipart", true) &lt;&gt; 0 Then
		///				' �Y�t�t�@�C����ۑ�
		///				imap.SaveMimePart(no, mime.PartNo, "z:\temp\" + mime.FileName)
		///			End If
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// MIME �p�[�g���擾
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <param name="part_no">MIME �p�[�g�ԍ�</param>
		/// <remarks>
		/// <para>���ǃt���O�𗧂ĂȂ��ꍇ�A�f�R�[�h���Ȃ��ꍇ�́A<see cref="MimeFlag"/>�Őݒ肵�Ă����܂��B</para>
		/// <para>�擾�����p�[�g�{����<see cref="Body"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�T���v���� <see cref="GetMimeStructure"/>���Q�Ƃ��Ă��������B</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// MIME �p�[�g�w�b�_���擾
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <param name="part_no">MIME �p�[�g�ԍ�</param>
		/// <remarks>
		/// <para>���ǃt���O�𗧂ĂȂ��ꍇ�A�f�R�[�h���Ȃ��ꍇ�́A<see cref="MimeFlag"/>�Őݒ肵�Ă����܂��B</para>
		/// <para>�擾�����p�[�g�w�b�_��<see cref="Header"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�T���v���� <see cref="GetMimeStructure"/>���Q�Ƃ��Ă��������B</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// MIME �p�[�g��ۑ�
		/// </summary>
		/// <param name="no">���[���ԍ�(<see cref="UseUid"/>�� true �̏ꍇ UID)</param>
		/// <param name="part_no">MIME �p�[�g�ԍ�</param>
		/// <param name="path">�ۑ�����t�@�C����</param>
		/// <remarks>
		/// <para>���ǃt���O�𗧂ĂȂ��ꍇ�A�f�R�[�h���Ȃ��ꍇ�́A<see cref="MimeFlag"/>�Őݒ肵�Ă����܂��B</para>
		/// <para>�ۑ������t�@�C������<see cref="FileName"/>�Ŏ擾�ł��܂��B</para>
		/// <para>�T���v���� <see cref="GetMimeStructure"/>���Q�Ƃ��Ă��������B</para>
		/// <para>Content-Type �̃^�C�v�� "multipart" �� MIME �p�[�g���w�肵���ꍇ�A�G���[�ɂȂ�܂��̂ł����Ӊ������B</para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���̌������s(������L)
		/// </summary>
		/// <param name="type">��������
		/// <para>���������̐ݒ�\�Ȓl�͉��L�̒ʂ�ł��B</para>
		/// <para><see cref="SearchKeyword"/>�w�肳�ꂽ�L�[���[�h����</para>
		/// <para><see cref="SearchUnKeyword"/>�w�肳�ꂽ�L�[���[�h�Ȃ�</para>
		/// <para><see cref="SearchFrom"/>From �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchTo"/>To �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchCc"/>CC �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchBcc"/>BCC �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchSubject"/>SUBJECT �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchSentBefore"/>DATE �w�b�_���w�肳�ꂽ���t�ȑO</para>
		/// <para><see cref="SearchSentOn"/>DATE �w�b�_���w�肳�ꂽ���t</para>
		/// <para><see cref="SearchSentSince"/>DATE �w�b�_���w�肳�ꂽ���t�ȍ~</para>
		/// <para><see cref="SearchBefore"/>�w�肳�ꂽ���t�ȑO�ɓ���</para>
		/// <para><see cref="SearchOn"/>�w�肳�ꂽ���t�ɓ���</para>
		/// <para><see cref="SearchSince"/>�w�肳�ꂽ���t�ȍ~�ɓ���</para>
		/// <para><see cref="SearchHeader"/>�w�肳�ꂽ�w�b�_�t�B�[���h�ɕ�������܂� �����������"�w�b�_�t�B�[���h ������"�Ǝw�肵�Ă�������</para>
		/// <para><see cref="SearchBody"/>�{���ɕ�������܂�</para>
		/// <para><see cref="SearchText"/>�w�b�_����і{���ɕ�������܂�</para>
		/// <para><see cref="SearchLager"/>�w��T�C�Y���傫�ȃ��[�� �T�C�Y�͕�����ɕϊ����Ă�������</para>
		/// <para><see cref="SearchSmaller"/>�w��T�C�Y��菬���ȃ��[�� �T�C�Y�͕�����ɕϊ����Ă�������</para>
		/// <para><see cref="SearchUid"/>�w�� UID �̃��[��</para>
		/// <para><see cref="SearchCommand"/>����������𒼐ڎw��</para>
		/// <para><see cref="SearchNot"/>�������� NOT (��L�̌��������Ɠ�����(C# �ł� | �AVB.NET �ł� Or)�w�肷��K�v������܂�)</para>
		/// </param>
		/// <param name="text">����������</param>
		/// <remarks>
		///	�������ʂ� <see cref="GetSearchMailResultList"/> �Ŏ擾�ł��܂��B
		/// <example>
		/// <para>
		///	���[���{�b�N�X("inbox")���̍��o�l(From �t�B�[���h)�� support@nanshiki.co.jp ���܂܂�郁�[������������
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
		///				MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.Search(SearchFrom, "support@nanshiki.co.jp")
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���̌������s(������)
		/// </summary>
		/// <param name="type">��������
		/// <para>���������̐ݒ�\�Ȓl�͉��L�̒ʂ�ł��B</para>
		/// <para><see cref="SearchAnswered"/>�ԐM�ς�</para>
		/// <para><see cref="SearchUnAnswered"/>�ԐM�ς݂łȂ�</para>
		/// <para><see cref="SearchDeleted"/>�����}�[�N����</para>
		/// <para><see cref="SearchUnDeleted"/>�����}�[�N�Ȃ�</para>
		/// <para><see cref="SearchDraft"/>���e�t���O����</para>
		/// <para><see cref="SearchUnDraft"/>���e�t���O�Ȃ�</para>
		/// <para><see cref="SearchFlagged"/>�d�v���t���O����</para>
		/// <para><see cref="SearchUnFlagged"/>�d�v���t���O�Ȃ�</para>
		/// <para><see cref="SearchRecent"/>�����t���O����</para>
		/// <para><see cref="SearchUnRecent"/>�����t���O�Ȃ�</para>
		/// <para><see cref="SearchSeen"/>���ǃt���O����</para>
		/// <para><see cref="SearchUnSeen"/>���ǃt���O�Ȃ�</para>
		/// <para><see cref="SearchNew"/>�����t���O���肩���ǃt���O�Ȃ�</para>
		/// <para><see cref="SearchAll"/>�S�Ẵ��[��</para>
		/// <para><see cref="SearchNot"/>�������� NOT (��L�̌��������Ɠ�����(C# �ł� | �AVB.NET �ł� Or)�w�肷��K�v������܂�)</para>
		/// </param>
		/// <remarks>
		///	�������ʂ� <see cref="GetSearchMailResultList"/> �Ŏ擾�ł��܂��B
		/// <example>
		/// <para>
		///	���[���{�b�N�X("inbox")���̐V�K�̃��[��(�����t���O�L�肩���ǃt���O�Ȃ�)����������
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
		///				MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.Search(SearchNew)
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void Search(int type)
		{
			Search(type, null);
		}

		/// <summary>
		/// ���[���̌��� AND (������L)
		/// </summary>
		/// <param name="type">��������
		/// <para>���������̐ݒ�\�Ȓl�͉��L�̒ʂ�ł��B</para>
		/// <para><see cref="SearchKeyword"/>�w�肳�ꂽ�L�[���[�h����</para>
		/// <para><see cref="SearchUnKeyword"/>�w�肳�ꂽ�L�[���[�h�Ȃ�</para>
		/// <para><see cref="SearchFrom"/>From �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchTo"/>To �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchCc"/>CC �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchBcc"/>BCC �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchSubject"/>SUBJECT �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchSentBefore"/>DATE �w�b�_���w�肳�ꂽ���t�ȑO</para>
		/// <para><see cref="SearchSentOn"/>DATE �w�b�_���w�肳�ꂽ���t</para>
		/// <para><see cref="SearchSentSince"/>DATE �w�b�_���w�肳�ꂽ���t�ȍ~</para>
		/// <para><see cref="SearchBefore"/>�w�肳�ꂽ���t�ȑO�ɓ���</para>
		/// <para><see cref="SearchOn"/>�w�肳�ꂽ���t�ɓ���</para>
		/// <para><see cref="SearchSince"/>�w�肳�ꂽ���t�ȍ~�ɓ���</para>
		/// <para><see cref="SearchHeader"/>�w�肳�ꂽ�w�b�_�t�B�[���h�ɕ�������܂� �����������"�w�b�_�t�B�[���h ������"�Ǝw�肵�Ă�������</para>
		/// <para><see cref="SearchBody"/>�{���ɕ�������܂�</para>
		/// <para><see cref="SearchText"/>�w�b�_����і{���ɕ�������܂�</para>
		/// <para><see cref="SearchLager"/>�w��T�C�Y���傫�ȃ��[�� �T�C�Y�͕�����ɕϊ����Ă�������</para>
		/// <para><see cref="SearchSmaller"/>�w��T�C�Y��菬���ȃ��[�� �T�C�Y�͕�����ɕϊ����Ă�������</para>
		/// <para><see cref="SearchUid"/>�w�� UID �̃��[��</para>
		/// <para><see cref="SearchCommand"/>����������𒼐ڎw��</para>
		/// <para><see cref="SearchNot"/>�������� NOT (��L�̌��������Ɠ�����(C# �ł� | �AVB.NET �ł� Or)�w�肷��K�v������܂�)</para>
		/// </param>
		/// <param name="text">����������</param>
		/// <remarks>
		///	AND �Ō����������w�肵�܂��B<see cref="Search"/>�Ō��������s���܂��B
		/// <example>
		/// <para>
		///	���[���{�b�N�X("inbox")���̌�����"�e�X�g"���܂܂�A���o�l(From �t�B�[���h)�� support@nanshiki.co.jp ���܂܂�郁�[������������
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.AddSearchAnd(SearchSubject, "�e�X�g");
		///			imap.Search(SearchFrom, "support@nanshiki.co.jp");
		///			uint [] list = imap4.GetSearchMailResultList()
		///			foreach(uint no in list)
		///			{
		///				MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.AddSearchAnd(SearchSubject, "�e�X�g")
		///		imap.Search(SearchFrom, "support@nanshiki.co.jp")
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���̌��� AND (������)
		/// </summary>
		/// <param name="type">��������
		/// <para>���������̐ݒ�\�Ȓl�͉��L�̒ʂ�ł��B</para>
		/// <para><see cref="SearchAnswered"/>�ԐM�ς�</para>
		/// <para><see cref="SearchUnAnswered"/>�ԐM�ς݂łȂ�</para>
		/// <para><see cref="SearchDeleted"/>�����}�[�N����</para>
		/// <para><see cref="SearchUnDeleted"/>�����}�[�N�Ȃ�</para>
		/// <para><see cref="SearchDraft"/>���e�t���O����</para>
		/// <para><see cref="SearchUnDraft"/>���e�t���O�Ȃ�</para>
		/// <para><see cref="SearchFlagged"/>�d�v���t���O����</para>
		/// <para><see cref="SearchUnFlagged"/>�d�v���t���O�Ȃ�</para>
		/// <para><see cref="SearchRecent"/>�����t���O����</para>
		/// <para><see cref="SearchUnRecent"/>�����t���O�Ȃ�</para>
		/// <para><see cref="SearchSeen"/>���ǃt���O����</para>
		/// <para><see cref="SearchUnSeen"/>���ǃt���O�Ȃ�</para>
		/// <para><see cref="SearchNew"/>�����t���O���肩���ǃt���O�Ȃ�</para>
		/// <para><see cref="SearchAll"/>�S�Ẵ��[��</para>
		/// <para><see cref="SearchNot"/>�������� NOT (��L�̌��������Ɠ�����(C# �ł� | �AVB.NET �ł� Or)�w�肷��K�v������܂�)</para>
		/// </param>
		/// <remarks>
		///	AND �Ō����������w�肵�܂��B<see cref="Search"/>�Ō��������s���܂��B
		/// <example>
		/// <para>
		///	���[���{�b�N�X("inbox")���̊��ǂ̃��[���� 2009/8/1 �ȍ~�ɓ����������[������������
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
		///				MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.AddSearchAnd(SearchSeen)
		///		imap.Search(SearchSince, MakeDateString(new DateTime(2009, 8, 1))
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void AddSearchAnd(int type)
		{
			AddSearchAnd(type, null);
		}
		/// <summary>
		/// ���[���̌��� OR (������L)
		/// </summary>
		/// <param name="type">��������
		/// <para>���������̐ݒ�\�Ȓl�͉��L�̒ʂ�ł��B</para>
		/// <para><see cref="SearchKeyword"/>�w�肳�ꂽ�L�[���[�h����</para>
		/// <para><see cref="SearchUnKeyword"/>�w�肳�ꂽ�L�[���[�h�Ȃ�</para>
		/// <para><see cref="SearchFrom"/>From �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchTo"/>To �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchCc"/>CC �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchBcc"/>BCC �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchSubject"/>SUBJECT �w�b�_�Ɏw��̕����񂠂�</para>
		/// <para><see cref="SearchSentBefore"/>DATE �w�b�_���w�肳�ꂽ���t�ȑO</para>
		/// <para><see cref="SearchSentOn"/>DATE �w�b�_���w�肳�ꂽ���t</para>
		/// <para><see cref="SearchSentSince"/>DATE �w�b�_���w�肳�ꂽ���t�ȍ~</para>
		/// <para><see cref="SearchBefore"/>�w�肳�ꂽ���t�ȑO�ɓ���</para>
		/// <para><see cref="SearchOn"/>�w�肳�ꂽ���t�ɓ���</para>
		/// <para><see cref="SearchSince"/>�w�肳�ꂽ���t�ȍ~�ɓ���</para>
		/// <para><see cref="SearchHeader"/>�w�肳�ꂽ�w�b�_�t�B�[���h�ɕ�������܂� �����������"�w�b�_�t�B�[���h ������"�Ǝw�肵�Ă�������</para>
		/// <para><see cref="SearchBody"/>�{���ɕ�������܂�</para>
		/// <para><see cref="SearchText"/>�w�b�_����і{���ɕ�������܂�</para>
		/// <para><see cref="SearchLager"/>�w��T�C�Y���傫�ȃ��[�� �T�C�Y�͕�����ɕϊ����Ă�������</para>
		/// <para><see cref="SearchSmaller"/>�w��T�C�Y��菬���ȃ��[�� �T�C�Y�͕�����ɕϊ����Ă�������</para>
		/// <para><see cref="SearchUid"/>�w�� UID �̃��[��</para>
		/// <para><see cref="SearchCommand"/>����������𒼐ڎw��</para>
		/// <para><see cref="SearchNot"/>�������� NOT (��L�̌��������Ɠ�����(C# �ł� | �AVB.NET �ł� Or)�w�肷��K�v������܂�)</para>
		/// </param>
		/// <param name="text">����������</param>
		/// <remarks>
		///	OR �Ō����������w�肵�܂��B<see cref="Search"/>�Ō��������s���܂��B
		/// <example>
		/// <para>
		///	���[���{�b�N�X("inbox")���̌�����"�e�X�g"���܂܂�邩�A�܂��͍��o�l(From �t�B�[���h)�� support@nanshiki.co.jp ���܂܂�郁�[������������
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.AddSearchOr(SearchSubject, "�e�X�g");
		///			imap.Search(SearchFrom, "support@nanshiki.co.jp");
		///			uint [] list = imap4.GetSearchMailResultList()
		///			foreach(uint no in list)
		///			{
		///				MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.AddSearchOr(SearchSubject, "�e�X�g")
		///		imap.Search(SearchFrom, "support@nanshiki.co.jp")
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���̌��� OR (������)
		/// </summary>
		/// <param name="type">��������
		/// <para>���������̐ݒ�\�Ȓl�͉��L�̒ʂ�ł��B</para>
		/// <para><see cref="SearchAnswered"/>�ԐM�ς�</para>
		/// <para><see cref="SearchUnAnswered"/>�ԐM�ς݂łȂ�</para>
		/// <para><see cref="SearchDeleted"/>�����}�[�N����</para>
		/// <para><see cref="SearchUnDeleted"/>�����}�[�N�Ȃ�</para>
		/// <para><see cref="SearchDraft"/>���e�t���O����</para>
		/// <para><see cref="SearchUnDraft"/>���e�t���O�Ȃ�</para>
		/// <para><see cref="SearchFlagged"/>�d�v���t���O����</para>
		/// <para><see cref="SearchUnFlagged"/>�d�v���t���O�Ȃ�</para>
		/// <para><see cref="SearchRecent"/>�����t���O����</para>
		/// <para><see cref="SearchUnRecent"/>�����t���O�Ȃ�</para>
		/// <para><see cref="SearchSeen"/>���ǃt���O����</para>
		/// <para><see cref="SearchUnSeen"/>���ǃt���O�Ȃ�</para>
		/// <para><see cref="SearchNew"/>�����t���O���肩���ǃt���O�Ȃ�</para>
		/// <para><see cref="SearchAll"/>�S�Ẵ��[��</para>
		/// <para><see cref="SearchNot"/>�������� NOT (��L�̌��������Ɠ�����(C# �ł� | �AVB.NET �ł� Or)�w�肷��K�v������܂�)</para>
		/// </param>
		/// <remarks>
		///	OR �Ō����������w�肵�܂��B<see cref="Search"/>�Ō��������s���܂��B
		/// <example>
		/// <para>
		///	���[���{�b�N�X("inbox")���̊��ǂ̃��[�����A�܂��͌�����"�e�X�g"���܂܂�郁�[������������
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.SelectMailBox("inbox");
		///			imap.AddSearchOr(SearchSeen);
		///			imap.Search(SearchSubject, "�e�X�g");
		///			uint [] list = imap4.GetSearchMailResultList()
		///			foreach(uint no in list)
		///			{
		///				MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.AddSearchOr(SearchSeen)
		///		imap.Search(SearchSubject, "�e�X�g")
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void AddSearchOr(int type)
		{
			AddSearchOr(type, null);
		}

		/// <summary>
		/// ���[���{�b�N�X�Ƀ��[�����A�b�v���[�h���܂��B
		/// </summary>
		/// <param name="mailbox">�A�b�v���[�h���郁�[���{�b�N�X</param>
		/// <param name="header">���[���w�b�_
		/// <para>Date,Message-ID,Mime-Version,Content-Type,Content-Transfer-Encode �t�B�[���h</para>
		/// <para>�͎����I�ɕt������܂��BDate �t�B�[���h�ɂ͌��ݓ������ݒ肳��܂��B</para>
		/// <para>Date ����� Message-ID �t�B�[���h�͈��� header �ɋL�q������΂����炪�D�悳��܂��B</para>
		/// </param>
		/// <param name="body">���[���{��</param>
		/// <param name="message_flag">���b�Z�[�W�t���O
		/// <para><see cref="MessageAnswerd"/>�ԐM�ς݃t���O</para>
		/// <para><see cref="MessageDeleted"/>�����}�[�N</para>
		/// <para><see cref="MessageDraft"/>���e�t���O</para>
		/// <para><see cref="MessageFlagged"/>�d�v���t���O</para>
		/// <para><see cref="MessageRecent"/>�����t���O</para>
		/// <para><see cref="MessageSeen"/>���ǃt���O</para>
		/// <para>C# �� | �AVB.NET �ł� Or �ŕ����w��\�ł��B</para>
		/// </param>
		/// <param name="path">�Y�t�t�@�C����</param>
		/// <remarks>
		/// <example>
		/// <para>
		///	���M�σ��[���{�b�N�X("sent")�Ƀ��[�������ǃt���O�t���ŃA�b�v���[�h����B
		/// subject �Ɍ����Afrom �ɍ��o�l�Ato �Ɉ���Abody �Ƀ��[���{���Apath �ɓY�t�t�@�C���̃p�X���������Ă�����̂Ƃ���B
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
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
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
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// ���[���{�b�N�X�Ƀ��[�����A�b�v���[�h���܂��B
		/// </summary>
		/// <param name="mailbox">�A�b�v���[�h���郁�[���{�b�N�X</param>
		/// <param name="header">���[���w�b�_
		/// <para>Date,Message-ID,Mime-Version,Content-Type,Content-Transfer-Encode �t�B�[���h</para>
		/// <para>�͎����I�ɕt������܂��BDate �t�B�[���h�ɂ͌��ݓ������ݒ肳��܂��B</para>
		/// <para>Date ����� Message-ID �t�B�[���h�͈��� header �ɋL�q������΂����炪�D�悳��܂��B</para>
		/// </param>
		/// <param name="body">���[���{��</param>
		/// <param name="message_flag">���b�Z�[�W�t���O
		/// <para><see cref="MessageAnswerd"/>�ԐM�ς݃t���O</para>
		/// <para><see cref="MessageDeleted"/>�����}�[�N</para>
		/// <para><see cref="MessageDraft"/>���e�t���O</para>
		/// <para><see cref="MessageFlagged"/>�d�v���t���O</para>
		/// <para><see cref="MessageRecent"/>�����t���O</para>
		/// <para><see cref="MessageSeen"/>���ǃt���O</para>
		/// <para>C# �� | �AVB.NET �ł� Or �ŕ����w��\�ł��B</para>
		/// </param>
		/// <remarks>
		/// <example>
		/// <para>
		///	���M�σ��[���{�b�N�X("sent")�Ƀ��[�������ǃt���O�t���ŃA�b�v���[�h����B
		/// subject �Ɍ����Afrom �ɍ��o�l�Ato �Ɉ���Abody �Ƀ��[���{���������Ă�����̂Ƃ���B
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
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
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
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
		/// </exception>
		public void AppendMail(string mailbox, string header, string body, int message_flag)
		{
			AppendMail(mailbox, header, body, message_flag, null);
		}

		/// <summary>
		/// ���O��Ԃ��擾���܂��B
		/// </summary>
		/// <remarks>
		/// <example>
		/// <para>
		///	���O��Ԃ��擾���A�\������B
		/// <code lang="cs">
		///	using(nMail.Imap4 imap = new nMail.Imap4("mail.example.com")) {
		///		string header;
		///		try {
		///			imap.Connect();
		///			imap.Authenticate("imap4_id", "password");
		///			imap.GetNameSpace();
		///			nMail.Imap4.NameSpaceStatus [] list = imap.GetNameSpacePersonalStatusList()
		///			foreach(nMail.Imap4.NameSpaceStatus item in list) {
		///				MessageBox.Show(string.Format("�l���O���:{0:s} ��؂蕶��:{1:c}", item.Name, item.Separate));
		///			}
		///			list = imap.GetNameSpaceOtherStatusList()
		///			foreach(nMail.Imap4.NameSpaceStatus item in list) {
		///				MessageBox.Show(string.Format("���l���O���:{0:s} ��؂蕶��:{1:c}", item.Name, item.Separate));
		///			}
		///			list = imap.GetNameSpaceSharedStatusList()
		///			foreach(nMail.Imap4.NameSpaceStatus item in list) {
		///				MessageBox.Show(string.Format("���L���O���:{0:s} ��؂蕶��:{1:c}", item.Name, item.Separate));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Dim header As String
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.GetNameSpace()
		///		Dim list As nMail.Imap4.NameSpaceStatus() = imap.GetNameSpacePersonalStatusList()
		///		For Each item As nMail.Imap4.NameSpaceStatus In list
		///			MessageBox.Show(String.Format("�l���O���:{0:s} ��؂蕶��:{1:c}", item.Name, item.Separate))
		///		Next
		///		list = imap.GetNameSpaceOtherStatusList()
		///		For Each item As nMail.Imap4.NameSpaceStatus In list
		///			MessageBox.Show(String.Format("���l���O���:{0:s} ��؂蕶��:{1:c}", item.Name, item.Separate))
		///		Next
		///		list = imap.GetNameSpaceSharedStatusList()
		///		For Each item As nMail.Imap4.NameSpaceStatus In list
		///			MessageBox.Show(String.Format("���L���O���:{0:s} ��؂蕶��:{1:c}", item.Name, item.Separate))
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///	�ڑ���Ԃł͂���܂���B(<see cref="Connect"/>���������Ă��Ȃ����A�Ăяo����Ă��܂���B
		///	</exception>
		///	<exception cref="nMailException">
		///	�T�[�o�Ƃ̌�M���ɃG���[���������܂����B
		/// <see cref="nMailException.Message"/>�ɃG���[���b�Z�[�W�A
		/// <see cref="nMailException.ErrorCode"/>�ɃG���[�R�[�h������܂��B
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
		/// �����p���t�w�蕶������쐬���܂��B
		/// </summary>
		/// <remarks>
		/// ���t�w�蕶������쐬���܂��B
		/// <para>�����p ��: "09-Jul-2009"</para>
		/// <example>
		/// <para>
		///	���[���{�b�N�X("inbox")���� 2009/8/1 �ɓ����������[������������
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
		///				MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///			}
		///		}
		///		catch(nMail.nMailException nex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message));
		///		}
		///		catch(Exception ex)
		///		{
		///			MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message));
		///		}
		///	}
		/// </code>
		/// <code lang="vbnet">
		///	' VB.NET 2005 �ȍ~�̏ꍇ�AC# �Ɠ��l�� using ���g�p�ł��܂��B
		///	Dim imap As nMail.Imap4 = New nMail.Imap4("mail.example.com")
		///	Try
		///		imap.Connect()
		///		imap.Authenticate("imap4_id", "password")
		///		imap.SelectMailBox("inbox")
		///		imap.Search(SearchOn, MakeSearchDateString(new DateTime(2009, 8, 1))
		///		Dim list As Integer() = imap4.GetSearchMailResultList()
		///		For Each no As Integer In list
		///			MessageBox.Show(String.Format("�������ʃ��[���ԍ� {0}", no));
		///		Next
		///	Catch nex As nMail.nMailException
		///		MessageBox.Show(String.Format("�G���[ �ԍ�:{0:d} ���b�Z�[�W:{1:s}", nex.ErrorCode, nex.Message))
		///	Catch ex As Exception
		///		MessageBox.Show(String.Format("�G���[ ���b�Z�[�W:{0:s}", ex.Message))
		///	Finally
		///		imap.Dispose()
		///	End Try
		/// </code>
		/// </para>
		/// </example>
		/// </remarks>
		/// <returns>�쐬�������t�w�蕶����</returns>
		public string MakeSearchDateString(DateTime tm)
		{
			StringBuilder _datestring = new StringBuilder(DateStringSize);
			MakeDateString(_datestring, tm.Year, tm.Month, tm.Day, tm.Hour, tm.Minute, tm.Second, DateImap4);
			return _datestring.ToString();
		}
		/// <summary>
		/// AppenMail�p�����w�蕶������쐬���܂��B
		/// </summary>
		/// <remarks>
		/// �����w�蕶������쐬���܂��B
		/// <para>AppendMail�p ��: "09-Jul-2009 15:10:30"</para>
		/// <para>�����̂Ƃ��� AppendMail �ŃA�b�v���[�h����ۂ̃T�[�o�������I�Ɏ������w��ɂ�</para>
		/// <para>�@Imap4 �N���X�̃��\�b�h�ł͑Ή����Ă��܂���B</para>
		/// <para>�@Date �w�b�_�t�B�[���h�̒l�� <see cref="MakeFieldDateString"/> �ō쐬���ĕt�����邱�Ƃ��ł��܂��B</para>
		/// </remarks>
		/// <returns>�쐬���������w�蕶����</returns>
		public string MakeAppendDateString(DateTime tm)
		{
			StringBuilder _datestring = new StringBuilder(DateStringSize);
			MakeDateString(_datestring, tm.Year, tm.Month, tm.Day, tm.Hour, tm.Minute, tm.Second, DateTimeImap4);
			return _datestring.ToString();
		}
		/// <summary>
		/// Date �w�b�_�t�B�[���h�p�����w�蕶������쐬���܂��B
		/// </summary>
		/// <remarks>
		/// �����w�蕶������쐬���܂��B
		/// <para>Date �w�b�_�t�B�[���h�p ��: "Thu, 9 Jul 2009 15:10:30 +09:00"</para>
		/// <para><see cref="AppendMail"/> �Ŏw��̓����� Date �w�b�_�t�B�[���h��t���������ꍇ�Ɏg�p���܂��B</para>
		/// </remarks>
		/// <returns>�쐬���������w�蕶����</returns>
		public string MakeFieldDateString(DateTime tm)
		{
			StringBuilder _datestring = new StringBuilder(DateStringSize);
			MakeDateString(_datestring, tm.Year, tm.Month, tm.Day, tm.Hour, tm.Minute, tm.Second, DateTimeSmtp);
			return _datestring.ToString();
		}
		/// <summary>
		/// �Ȃɂ����s���܂���
		/// </summary>
		/// <remarks>
		/// �T�[�o�Ƃ̐ڑ��^�C���A�E�g�h�~���Ɏg�p���܂��B
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
		/// �Y�t�t�@�C�����̔z����擾���܂��B
		/// </summary>
		/// <returns>�Y�t�t�@�C�����̔z��</returns>
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
		/// ���[���{�b�N�X���̔z����擾���܂��B
		/// </summary>
		/// <returns>���[���{�b�N�X���̔z��</returns>
		public MailBoxStatus[] GetMailBoxStatusList()
		{
			return (MailBoxStatus[])_mailbox_list.Clone();
		}
		/// <summary>
		/// MIME �\���̔z����擾���܂��B
		/// </summary>
		/// <returns>MIME �\�����̔z��</returns>
		public MimeStructureStatus[] GetMimeStructureStatusList()
		{
			return (MimeStructureStatus[])_mime_list.Clone();
		}
		/// <summary>
		/// �������ʂ̔z����擾���܂��B
		/// </summary>
		/// <returns>�������ʂ̔z��</returns>
		public uint[] GetSearchMailResultList()
		{
			return (uint[])_search_list.Clone();
		}
		/// <summary>
		/// �l���O��Ԃ̔z����擾���܂��B
		/// </summary>
		/// <remarks>
		///	<para><see cref="GetNameSpace"/>���s��ɌĂяo���K�v������܂��B</para>
		/// </remarks>
		/// <returns>�l���O��ԏ��̔z��</returns>
		public NameSpaceStatus[] GetNameSpacePersonalStatusList()
		{
			return (NameSpaceStatus[])_namespace_list[NameSpacePersonal].Clone();
		}
		/// <summary>
		/// ���l���O��Ԃ̔z����擾���܂��B
		/// </summary>
		/// <remarks>
		///	<para><see cref="GetNameSpace"/>���s��ɌĂяo���K�v������܂��B</para>
		/// </remarks>
		/// <returns>���l���O��ԏ��̔z��</returns>
		public NameSpaceStatus[] GetNameSpaceOtherStatusList()
		{
			return (NameSpaceStatus[])_namespace_list[NameSpaceOther].Clone();
		}
		/// <summary>
		/// ���L���O��Ԃ̔z����擾���܂��B
		/// </summary>
		/// <remarks>
		///	<para><see cref="GetNameSpace"/>���s��ɌĂяo���K�v������܂��B</para>
		/// </remarks>
		/// <returns>���L���O��ԏ��̔z��</returns>
		public NameSpaceStatus[] GetNameSpaceSharedStatusList()
		{
			return (NameSpaceStatus[])_namespace_list[NameSpaceShared].Clone();
		}

		/// <summary>
		/// IMAP4 �|�[�g�ԍ��ł��B
		/// </summary>
		/// <value>IMAP4 �|�[�g�ԍ�</value>
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
		/// �\�P�b�g�n���h���ł��B
		/// </summary>
		/// <value>�\�P�b�g�n���h��</value>
		public IntPtr Handle {
			get
			{
				return _socket;
			}
		}
		/// <summary>
		/// ���[���{�b�N�X�ɂ��郁�[�����ł��B
		/// </summary>
		/// <value>���ݑI������Ă��郁�[���{�b�N�X�ɂ��郁�[����</value>
		public int Count {
			get
			{
				return _count;
			}
		}
		/// <summary>
		/// ���[���̃T�C�Y�ł��B
		/// </summary>
		/// <value>���[���̃T�C�Y</value>
		public int Size {
			get
			{
				return _size;
			}
		}
		/// <summary>
		/// IMAP4 �T�[�o���ł��B
		/// </summary>
		/// <value>IMAP4 �T�[�o��</value>
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
		/// IMAP4 ���[�U�[���ł��B
		/// </summary>
		/// <value>IMAP4 ���[�U�[��</value>
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
		/// IMAP4 �p�X���[�h�ł��B
		/// </summary>
		/// <value>IMAP4 �p�X���[�h</value>
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
		/// �Y�t�t�@�C����ۑ�����t�H���_�ł��B
		/// </summary>
		/// <remarks>
		/// null (VB.Net �� nothing) �̏ꍇ�ۑ����܂���B
		/// </remarks>
		/// <value>�Y�t�t�@�C���ۑ��t�H���_</value>
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
		/// ���[���̖{���ł��B
		/// </summary>
		/// <value>���[���{��</value>
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
		/// ���[���̌����ł��B
		/// </summary>
		/// <value>���[���̌���</value>
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
		/// ���[���̑��M�����̕�����ł��B
		/// </summary>
		/// <value>���[�����M����������</value>
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
		/// ���[���̍��o�l�ł��B
		/// </summary>
		/// <value>���[���̍��o�l</value>
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
		/// ���[���̃w�b�_�ł��B
		/// </summary>
		/// <value>���[���̃w�b�_</value>
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
		/// �Y�t�t�@�C�����ł��B
		/// </summary>
		/// <remarks>
		/// �����̓Y�t�t�@�C��������ꍇ�A"," �ŋ�؂��Ċi�[����܂��B
		/// <see cref="Options.SplitChar"/>�ŋ�؂蕶����ύX�ł��܂��B
		/// </remarks>
		/// <value>�Y�t�t�@�C����</value>
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
		/// �Y�t�t�@�C�����̔z��ł��B
		/// </summary>
		///	<remarks>
		/// ���̃v���p�e�B�͌݊����̂��߂Ɏc���Ă���܂��B
		///	<see cref="GetFileNameList"/>�Ŕz����擾���Ďg�p����悤�ɂ��Ă��������B
		///	</remarks>
		/// <value>�Y�t�t�@�C�����̔z��</value>
		public string[] FileNameList
		{
			get
			{
				return _filename_list;
			}
		}
		/// <summary>
		/// ���[���{�b�N�X���̔z��
		/// </summary>
		///	<remarks>
		///	<see cref="GetMailBoxStatusList"/>�Ŕz����擾���Ďg�p����̂𐄑E���܂��B
		///	</remarks>
		/// <value>���[���{�b�N�X���̔z��</value>
		public MailBoxStatus[] MailBoxStatusList
		{
			get
			{
				return _mailbox_list;
			}
		}
		/// <summary>
		/// MIME �\�����̔z��
		/// </summary>
		///	<remarks>
		///	<see cref="GetMimeStructureStatusList"/>�Ŕz����擾���Ďg�p����̂𐄑E���܂��B
		///	</remarks>
		/// <value>MIME �\�����̔z��</value>
		public MimeStructureStatus[] MimeStructureStatusList
		{
			get
			{
				return _mime_list;
			}
		}
		/// <summary>
		/// �������ʂ̔z��
		/// </summary>
		///	<remarks>
		///	<see cref="GetSearchMailResultList"/>�Ŕz����擾���Ďg�p����̂𐄑E���܂��B
		///	</remarks>
		/// <value>�������ʂ̔z��</value>
		public uint[] SearchMailResultList
		{
			get
			{
				return _search_list;
			}
		}
		/// <summary>
		/// �l���O��ԏ��̔z��
		/// </summary>
		///	<remarks>
		///	<see cref="GetNameSpacePersonalStatusList"/>�Ŕz����擾���Ďg�p����̂𐄑E���܂��B
		///	</remarks>
		/// <value>�l���O��ԏ��̔z��</value>
		public NameSpaceStatus[] NameSpacePersonalStatusList
		{
			get
			{
				return _namespace_list[NameSpacePersonal];
			}
		}
		/// <summary>
		/// ���l���O��ԏ��̔z��
		/// </summary>
		///	<remarks>
		///	<see cref="GetNameSpaceOtherStatusList"/>�Ŕz����擾���Ďg�p����̂𐄑E���܂��B
		///	</remarks>
		/// <value>�l���O��ԏ��̔z��</value>
		public NameSpaceStatus[] NameSpaceOtherStatusList
		{
			get
			{
				return _namespace_list[NameSpaceOther];
			}
		}
		/// <summary>
		/// ���L���O��ԏ��̔z��
		/// </summary>
		///	<remarks>
		///	<see cref="GetNameSpaceSharedStatusList"/>�Ŕz����擾���Ďg�p����̂𐄑E���܂��B
		///	</remarks>
		/// <value>�l���O��ԏ��̔z��</value>
		public NameSpaceStatus[] NameSpaceSharedStatusList
		{
			get
			{
				return _namespace_list[NameSpaceShared];
			}
		}
		/// <summary>
		/// �w�b�_�̃t�B�[���h���ł��B
		/// </summary>
		/// <value>�w�b�_�̃t�B�[���h��</value>
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
		/// �w�b�_�t�B�[���h�̓��e�ł��B
		/// </summary>
		/// <value>�w�b�_�t�B�[���h�̓��e</value>
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
		/// ���[���� UID �ł��B
		/// </summary>
		/// <value>���[�� UID</value>
		public uint Uid
		{
			get
			{
				return _uid;
			}
		}
		/// <summary>
		/// IMAP4 �F�ؕ��@�ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="AuthLogin"/> �� LOGIN ���g�p���܂��B</para>
		/// <para><see cref="AuthPlain"/> �� PLAIN ���g�p���܂��B</para>
		/// <para><see cref="AuthCramMd5"/> CRAM MD5 ���g�p���܂��B</para>
		/// <para><see cref="AuthDigestMd5"/> DIGEST MD5 ���g�p���܂��B</para>
		/// <para>C# �� | �AVisual Basic �ł� or �ŕ����ݒ�\�ł��B</para>
		/// <para>DIGEST MD5->CRAM MD5��PLAIN��LOGIN �̏��ԂŔF�؂����݂܂��B</para>
		///	<para>�f�t�H���g�l�� AuthLogin �ł��B</para>
		/// </remarks>
		/// <value>IMAP4 �̔F�ؕ��@</value>
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
		/// ���[���g���@�\�w��t���O�ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="SuspendAttachmentFile"/>�Y�t�t�@�C����M�ňꎞ�x�~����̈��ڂɎw�肵�܂��B</para>
		/// <para><see cref="SuspendNext"/>�Y�t�t�@�C����M�ňꎞ�x�~����̓��ڈȍ~�Ɏw�肵�܂��B</para>
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
		/// MIME �p�[�g�擾�������͕ۑ����̐ݒ�t���O�ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="MimePeek"/>���ǃt���O�𗧂Ă܂���B</para>
		/// <para><see cref="MimeNoDecode"/>�f�R�[�h���܂���B</para>
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
		/// ���[�����b�Z�[�W�̃t���O�ł��B
		/// </summary>
		/// <remarks>
		/// <para><see cref="MessageAnswerd"/>�ԐM�ς݃t���O���t���Ă��܂��B</para>
		/// <para><see cref="MessageDeleted"/>�����}�[�N���t���Ă��܂��B</para>
		/// <para><see cref="MessageDraft"/>���e�t���O���t���Ă��܂��B</para>
		/// <para><see cref="MessageFlagged"/>�d�v���t���O���t���Ă��܂��B</para>
		/// <para><see cref="MessageRecent"/>�����t���O���t���Ă��܂��B</para>
		/// <para><see cref="MessageSeen"/>���ǃt���O���t���Ă��܂��B</para>
		/// </remarks>
		public int MessageFlag
		{
			get
			{
				return _message_flag;
			}
		}
		/// <summary>
		/// ���[�����w�肷��ۂ� UID ���g�p���邩�ǂ����̃t���O�ł��B
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
		/// �G���[�ԍ��ł��B
		/// </summary>
		/// <value>�G���[�ԍ�</value>
		public int ErrorCode
		{
			get
			{
				return _err;
			}
		}
		/// <summary>
		/// text/html �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		/// <value>�t�@�C����</value>
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
		/// SSL �ݒ�t���O�ł��B
		/// </summary>
		/// <value>SSL �ݒ�t���O</value>
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
		/// SSL �N���C�A���g�ؖ������ł��B
		/// </summary>
		/// <remarks>
		/// null �̏ꍇ�w�肵�܂���B
		/// </remarks>
		/// <value>SSL �N���C�A���g�ؖ�����</value>
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
		/// message/rfc822 �p�[�g��ۑ������t�@�C���̖��O�ł��B
		/// </summary>
		/// <value>�t�@�C����</value>
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
	DLL �錾��

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

