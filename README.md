Akane Mail Version 1.2.1
========

Licence     : Modified BSD Licence
Platform    : WindowsXP(SP3), WindowsVista(SP2), Windows7(SP1), Windows Server2003/2008
Discription : This application is a Windows e-mail client software that supports POP3/APOP.

***** 注意 *****

Akane Mailのソースコードは修正BSDライセンスの元に公開をさせて頂いています。

・ソースコードは無保証であること。
・ソースコード等の著作権表示を削除しない条件の下での再配布許可。
・ソースコード、リソースの改変は私が著作権を持たないnMail.cs以外全てとする。
・著作権者の条件を付け加えることが可能。(改変したソースコードの公開を必須とする)

github版のソースコードには実行に必要なnMail.dllをレポジトリに格納していません。
別途以下のサイトよりnMail.dll開発セット(32bitおよび64bit)をダウンロードし
AkaneMailおよびMailConverterのディレクトリにnMail.dllをコピーしてください。

http://www.nanshiki.co.jp/software/index.html?nmail

* nMail.dll
  AkaneMail
  <Checkout Dir>\AkaneMail\AkaneMail\bin\<Debug/Release>\nMail.dll

  MailConverter
  <Checkout Dir>\MailConvert\MailConvert\bin\<Debug/Release>\nMail.dll

全てのソリューションのビルドが完了したらAkaneMailのbin\Release(Debugの場合はDebug)の
ディレクトリに以下のファイルを格納してから実行を開始してください。

* ACryptLib.dll
  <Checkout Dir>\ACryptLib\ACryptLib\bin\<Debug/Release>\ACryptLib.dll

* MailConvert.exe
  <Checkout Dir>\MailConvert\MailConvert\bin\<Debug/Release>MailConvert.exe

* <Checkout Dir>\AkaneMail\AkaneMail\bin\<Debug/Release>\に格納されるファイル
  AkaneMail.exe
  ACryptLib.dll
  MailConvert.exe
  nMail.dll

すぐにソースをビルドしたい場合はホームページに公開されているソースコード版をダウンロードして
ビルドしてください。(http://www.angel-teatime.com/soft/akane.htm)

なお、AkaneMailディレクトリのreadme.txtファイルは現状ZIP版と同じShiftJISコードとなっているため
GitHubのレポジトリから編集しようとすると日本語部分が文字化けするので直接編集はお控えください。

ご不便をお掛け致しますが、ご理解とご協力をよろしくお願い致します。

2013.07.16 Sakura Mizuki(Angelic Software)
URL   ：http://www.angel-teatime.com/


