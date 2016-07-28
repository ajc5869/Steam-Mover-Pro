Imports System.IO

Public Class Form1

    Public Path1toPath2 As New ArrayList
    Public Path2toPath1 As New ArrayList
    Public gigabyte_size As New Single

    Public transfer_to_folder_size As New Double
    Public transfer_from_folder_size As New Double
    Public bleh As New Double
    Public TRANSFER_COLOR As Color = Color.SkyBlue
    Public BG_COLOR As Color = Color.White
    Public itemsToMove As New ArrayList
    Public pathToCheck As String
    Dim timerThread As System.Threading.Thread
    Public full_command As String
    Dim copy_thread As System.Threading.Thread

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Pops up the folder select dialog box
        FolderBrowserDialog1.ShowNewFolderButton = False
        FolderBrowserDialog1.ShowDialog()

        If FolderBrowserDialog1.SelectedPath = "" Then

        Else
            Path1.Text = FolderBrowserDialog1.SelectedPath

            Path1toPath2.Clear()
            ListView1.Items.Clear()

            'Adds Path1 folder contents to the first listbox
            For Each folder As String In System.IO.Directory.GetDirectories(Path1.Text)
                Dim attrs As FileAttributes
                Dim dInfo As New DirectoryInfo(folder)
                If attrs And FileAttributes.ReparsePoint Then
                Else

                    folder = folder.Remove(0, Len(Path1.Text) + 1)
                    Dim lvi As New ListViewItem
                    lvi.Text = folder

                    ' set bool parameter to false if you
                    ' do not want to include subdirectories.
                    Dim sizeOfDir As Long = DirectorySize(dInfo, True)

                    gigabyte_size = sizeOfDir / (1024 ^ 3)
                    lvi.SubItems.Add(Math.Round(gigabyte_size, 2).ToString)

                    ListView1.Items.Add(lvi)
                End If
            Next

            Dim drive_letter As System.IO.DriveInfo
            drive_letter = My.Computer.FileSystem.GetDriveInfo(Path1.Text.Substring(0, 3))
            Label1.Text = CStr(Math.Round((drive_letter.TotalFreeSpace) / (1024 ^ 3), 2)) + " GB Free"
            Label1.Visible = True

        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Pops up the folder select dialog box
        FolderBrowserDialog2.ShowNewFolderButton = False
        FolderBrowserDialog2.ShowDialog()

        If FolderBrowserDialog2.SelectedPath = "" Then

        Else
            Path2.Text = FolderBrowserDialog2.SelectedPath

            Path2toPath1.Clear()
            ListView2.Items.Clear()

            'Adds Path2 folder contents to the second listbox
            For Each folder As String In System.IO.Directory.GetDirectories(Path2.Text)
                Dim attrs As FileAttributes
                Dim dInfo As New DirectoryInfo(folder)
                If attrs And FileAttributes.ReparsePoint Then
                Else
                    folder = folder.Remove(0, Len(Path2.Text) + 1)
                    Dim lvi As New ListViewItem(folder)


                    ' set bool parameter to false if you
                    ' do not want to include subdirectories.
                    Dim sizeOfDir As Long = DirectorySize(dInfo, True)

                    gigabyte_size = sizeOfDir / (1024 ^ 3)
                    lvi.SubItems.Add(Math.Round(gigabyte_size, 2).ToString)
                    ListView2.Items.Add(lvi)
                End If
            Next

            Dim drive_letter As System.IO.DriveInfo
            drive_letter = My.Computer.FileSystem.GetDriveInfo(Path2.Text.Substring(0, 3))
            Label2.Text = CStr(Math.Round((drive_letter.TotalFreeSpace) / (1024 ^ 3), 2)) + " GB Free"
            Label2.Visible = True
        End If

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim settingWhite As Integer = 0
        If String.IsNullOrWhiteSpace(Path2.Text) Or String.IsNullOrWhiteSpace(Path1.Text) Then
            MessageBox.Show("Need to specify source and destination path", "", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else

            Dim SelectedItem As New ListViewItem
            For Each SelectedItem In ListView1.SelectedItems
                Dim thingy As New ListViewItem
                thingy.Text = SelectedItem.Text
                thingy.SubItems.Add(SelectedItem.SubItems(1).Text)
                ListView2.Items.Add(thingy)
                If SelectedItem.BackColor = TRANSFER_COLOR Then
                    thingy.BackColor = BG_COLOR
                    Path2toPath1.Remove(Path2.Text + "\" + SelectedItem.Text)
                Else
                    thingy.BackColor = TRANSFER_COLOR
                    Path1toPath2.Add(Path1.Text + "\" + SelectedItem.Text)
                End If

                ListView1.Items.Remove(SelectedItem)




            Next
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        If String.IsNullOrWhiteSpace(Path2.Text) Or String.IsNullOrWhiteSpace(Path1.Text) Then
            MessageBox.Show("Need to specify source and destination path", "", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Dim itemsToMove As New ArrayList

            Dim SelectedItem As New ListViewItem
            For Each SelectedItem In ListView2.SelectedItems

                itemsToMove.Add(SelectedItem.Text)

                Dim thingy As New ListViewItem
                thingy.Text = SelectedItem.Text
                thingy.SubItems.Add(SelectedItem.SubItems(1).Text)
                If SelectedItem.BackColor = TRANSFER_COLOR Then
                    thingy.BackColor = Color.White
                    Path1toPath2.Remove(Path1.Text + "\" + SelectedItem.Text)
                Else SelectedItem.BackColor = Color.White
                    Path2toPath1.Add(Path2.Text + "\" + SelectedItem.Text)
                    thingy.BackColor = TRANSFER_COLOR
                End If
                ListView1.Items.Add(thingy)

                ListView2.Items.Remove(SelectedItem)
            Next

        End If
    End Sub



    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

        If Path1toPath2.Count = 0 And Path2toPath1.Count = 0 Then
            MessageBox.Show("There is nothing to transfer", "", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Dim totalNumber As Integer = Path1toPath2.Count() + Path2toPath1.Count()
            Dim count As Integer = 0

            MsgBox(totalNumber)


            transfer_to_folder_size = 0
            transfer_from_folder_size = 0

            Dim dInfo As New DirectoryInfo(Path2.Text)
            transfer_to_folder_size = DirectorySize(dInfo, True)


            For Each item In Path1toPath2
                Dim dInfo2 As New DirectoryInfo(item)
                'MsgBox(item)
                transfer_from_folder_size += DirectorySize(dInfo2, True)
            Next



            Dim dInfo3 As New DirectoryInfo(Path2.Text)
            transfer_to_folder_size = DirectorySize(dInfo3, True)

            Dim BlueItem As New ListViewItem
            For Each BlueItem In ListView1.Items
                If BlueItem.BackColor = Color.SkyBlue Then
                    transfer_from_folder_size += BlueItem.SubItems(1).Text
                End If
            Next

            For Each item In Path1toPath2
                count += 1
                Label3.Text = count.ToString + "/" + totalNumber.ToString + " " + item.Remove(0, Len(Path1.Text) - 1) + " to " + Path2.Text


                Dim counter As Integer = 0
                While counter <= 2
                    If counter = 0 Then
                        full_command = " /c xcopy /E /V /I /F /Y " + """" + item + """" + " " + """" + Path2.Text + "\" + item.Remove(0, Len(Path1.Text) + 1) + """"
                    End If

                    If counter = 1 Then
                        full_command = " /c rd /S /Q " + """" + item + """"
                    End If

                    If counter = 2 Then
                        full_command = " /c mklink /J " + """" + item + """" + " " + """" + Path2.Text + "\" + item.Remove(0, Len(Path1.Text) + 1) + """"
                    End If

                    Dim CopyStart As ProcessStartInfo = New ProcessStartInfo
                    Dim start As Process = New Process
                    CopyStart.CreateNoWindow = True
                    CopyStart.WindowStyle = ProcessWindowStyle.Hidden

                    CopyStart.FileName = "cmd.exe"
                    CopyStart.Arguments = full_command
                    CopyStart.UseShellExecute = False
                    start = Process.Start(CopyStart)
                    start.WaitForExit()
                    counter += 1
                End While
            Next
            Path1toPath2.Clear()



            For Each item In Path2toPath1
                count += 1
                Label3.Text = count.ToString + "/" + totalNumber.ToString + " " + item.Remove(0, Len(Path2.Text) - 1) + " to " + Path1.Text

                Dim counter As Integer = 0
                While counter <= 2
                    If counter = 0 Then
                        full_command = " /c xcopy /E /V /I /F /Y " + """" + item + """" + " " + """" + Path1.Text + "\" + item.Remove(0, Len(Path2.Text) + 1) + """"

                    End If

                    If counter = 1 Then
                        full_command = " /c rd /S /Q " + """" + item + """"
                    End If

                    If counter = 2 Then
                        full_command = " /c mklink /J " + """" + item + """" + " " + """" + Path1.Text + "\" + item.Remove(0, Len(Path2.Text) + 1) + """"
                    End If

                    Dim CopyStart As ProcessStartInfo = New ProcessStartInfo
                    Dim start As Process = New Process
                    CopyStart.CreateNoWindow = True
                    CopyStart.WindowStyle = ProcessWindowStyle.Hidden

                    CopyStart.FileName = "cmd.exe"
                    CopyStart.Arguments = full_command
                    CopyStart.UseShellExecute = False
                    start = Process.Start(CopyStart)
                    start.WaitForExit()
                    counter += 1
                End While
            Next
            Path2toPath1.Clear()
        End If

    End Sub

    Private Function DirectorySize(ByVal dInfo As DirectoryInfo,
   ByVal includeSubDir As Boolean) As Long
        ' Enumerate all the files
        Dim totalSize As Long = dInfo.EnumerateFiles() _
          .Sum(Function(file) file.Length)

        ' If Subdirectories are to be included
        If includeSubDir Then
            ' Enumerate all sub-directories
            totalSize += dInfo.EnumerateDirectories() _
             .Sum(Function(dir) DirectorySize(dir, True))
        End If
        Return totalSize
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListView1.GridLines = True
        ListView2.GridLines = True

        Path1.Text = My.Settings.Path1
        Dim driveLetter As String = Mid(Environment.GetFolderPath(Environment.SpecialFolder.System), 1, 3)
        'Dim pathToCheck As String

        If Environment.Is64BitOperatingSystem = True Then
            pathToCheck = driveLetter + "Program Files (x86)\Steam\steamapps\common"
        Else
            pathToCheck = driveLetter + "Program Files\Steam\steamapps\common"
        End If


        If Directory.Exists(pathToCheck) Then
            Path1.Text = pathToCheck

            Path1toPath2.Clear()
            ListView1.Items.Clear()

            'Adds Path1 folder contents to the first listbox
            For Each folder As String In System.IO.Directory.GetDirectories(Path1.Text)
                Dim attrs As FileAttributes
                Dim dInfo As New DirectoryInfo(folder)
                If attrs And FileAttributes.ReparsePoint Then
                Else
                    folder = folder.Remove(0, Len(Path1.Text) + 1)
                    Dim lvi As New ListViewItem
                    lvi.Text = folder

                    ' set bool parameter to false if you
                    ' do not want to include subdirectories.
                    Dim sizeOfDir As Long = DirectorySize(dInfo, True)

                    gigabyte_size = sizeOfDir / (1024 ^ 3)
                    lvi.SubItems.Add(Math.Round(gigabyte_size, 2).ToString)

                    ListView1.Items.Add(lvi)
                End If

            Next

            Dim drive_letter As System.IO.DriveInfo
            drive_letter = My.Computer.FileSystem.GetDriveInfo(Path1.Text.Substring(0, 3))
            Label1.Text = CStr(Math.Round((drive_letter.TotalFreeSpace) / (1024 ^ 3), 2)) + " GB Free"
            Label1.Visible = True
        Else
            MsgBox("Steam installation not detected, please manually locate your games")

        End If

    End Sub


    Private Sub Form1_FormClosing(sender As Object, e As EventArgs) Handles MyBase.FormClosing
        My.Settings.Path1 = Path1.Text
        My.Settings.Path2 = Path2.Text
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs)
        transfer_to_folder_size = 0
        transfer_from_folder_size = 0

        Dim dInfo As New DirectoryInfo(Path2.Text)
        transfer_to_folder_size = DirectorySize(dInfo, True)


        For Each item In Path1toPath2
            Dim dInfo2 As New DirectoryInfo(item)
            'MsgBox(item)
            transfer_from_folder_size += DirectorySize(dInfo2, True)
        Next

        bleh = transfer_from_folder_size
        MsgBox(bleh)
        Timer1.Enabled = True
        Timer1.Start()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'MsgBox(bleh)
        Dim dInfo As New DirectoryInfo(Path2.Text)
        Dim current_transfer_size As New Double
        current_transfer_size = DirectorySize(dInfo, True)
        Dim firstThing As New Integer
        Dim secondThing As New Integer

        bleh = bleh + 1

        Dim temp As Double
        firstThing = current_transfer_size - transfer_to_folder_size
        secondThing = firstThing / bleh
        temp = secondThing
        MsgBox(temp)
        ProgressBar1.Value = temp * 100
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub Path1_TextChanged(sender As Object, e As EventArgs) Handles Path1.TextChanged

    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged

    End Sub
End Class
