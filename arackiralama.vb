Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Windows.Forms

Public Class Form1
    Inherits Form ' BU SATIR EKSİKTİ: Form özellikleri için gereklidir.

    Private connStr As String = "Server=localhost\SQLEXPRESS01;Database=CarRentalSystem;Integrated Security=true;TrustServerCertificate=true;"
    Private dgv As DataGridView
    Private lbl As Label
    Private pnl As Panel
    Private txt1 As TextBox
    Private txt2 As TextBox
    Private txt3 As TextBox
    Private txt4 As TextBox
    Private txt5 As TextBox
    Private txt6 As TextBox
    Private isUpdateMode As Boolean = False
    Private updateVehicleId As Integer = 0

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Araç Kiralama Sistemi"
        Me.Size = New Size(1000, 700)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.WhiteSmoke
        CreateUI()
        TestDB()
    End Sub

    Private Sub CreateUI()
        Dim btnPanel As New Panel
        btnPanel.Location = New Point(10, 10)
        btnPanel.Size = New Size(970, 110)
        btnPanel.BackColor = Color.White
        btnPanel.BorderStyle = BorderStyle.FixedSingle

        CreateButton(btnPanel, "Müşteriler", 10, 10, Color.CornflowerBlue, AddressOf BtnCustomers_Click)
        CreateButton(btnPanel, "Tüm Araçlar", 250, 10, Color.MediumSeaGreen, AddressOf BtnAllVehicles_Click)
        CreateButton(btnPanel, "Uygun Araçlar", 490, 10, Color.LimeGreen, AddressOf BtnAvailable_Click)
        CreateButton(btnPanel, "Kiralamalar", 730, 10, Color.Gold, AddressOf BtnRentals_Click)

        CreateButton(btnPanel, "Araç Ekle", 10, 55, Color.Tomato, AddressOf BtnAdd_Click)
        CreateButton(btnPanel, "Araç Güncelle", 250, 55, Color.MediumPurple, AddressOf BtnUpdate_Click)
        CreateButton(btnPanel, "Araç Sil", 490, 55, Color.OrangeRed, AddressOf BtnDelete_Click)
        CreateButton(btnPanel, "Araç Kirala", 730, 55, Color.DeepSkyBlue, AddressOf BtnRent_Click)

        Me.Controls.Add(btnPanel)

        dgv = New DataGridView
        dgv.Location = New Point(10, 130)
        dgv.Size = New Size(970, 470)
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgv.ReadOnly = True
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.BackgroundColor = Color.White
        dgv.AllowUserToAddRows = False
        Me.Controls.Add(dgv)

        lbl = New Label
        lbl.Location = New Point(10, 610)
        lbl.Size = New Size(970, 40)
        lbl.Font = New Font("Arial", 11, FontStyle.Bold)
        lbl.BackColor = Color.LightYellow
        lbl.BorderStyle = BorderStyle.FixedSingle
        lbl.Text = "  Hazır..."
        Me.Controls.Add(lbl)

        pnl = New Panel
        pnl.Location = New Point(300, 150)
        pnl.Size = New Size(400, 450)
        pnl.BackColor = Color.LemonChiffon
        pnl.BorderStyle = BorderStyle.FixedSingle
        pnl.Visible = False

        Dim lblTitle As New Label
        lblTitle.Name = "lblTitle"
        lblTitle.Text = "YENİ ARAÇ EKLE"
        lblTitle.Location = New Point(10, 10)
        lblTitle.Size = New Size(380, 40)
        lblTitle.Font = New Font("Arial", 14, FontStyle.Bold)
        lblTitle.ForeColor = Color.DarkGreen
        lblTitle.TextAlign = ContentAlignment.MiddleCenter
        lblTitle.BackColor = Color.LightGreen
        pnl.Controls.Add(lblTitle)

        Dim lbl1 As New Label
        lbl1.Text = "Marka:"
        lbl1.Location = New Point(20, 65)
        lbl1.Size = New Size(360, 20)
        lbl1.Font = New Font("Arial", 9, FontStyle.Bold)
        pnl.Controls.Add(lbl1)

        txt1 = New TextBox
        txt1.Location = New Point(20, 85)
        txt1.Size = New Size(360, 25)
        txt1.Font = New Font("Arial", 10)
        pnl.Controls.Add(txt1)

        Dim lbl2 As New Label
        lbl2.Text = "Model:"
        lbl2.Location = New Point(20, 120)
        lbl2.Size = New Size(360, 20)
        lbl2.Font = New Font("Arial", 9, FontStyle.Bold)
        pnl.Controls.Add(lbl2)

        txt2 = New TextBox
        txt2.Location = New Point(20, 140)
        txt2.Size = New Size(360, 25)
        txt2.Font = New Font("Arial", 10)
        pnl.Controls.Add(txt2)

        Dim lbl3 As New Label
        lbl3.Text = "Yıl:"
        lbl3.Location = New Point(20, 175)
        lbl3.Size = New Size(360, 20)
        lbl3.Font = New Font("Arial", 9, FontStyle.Bold)
        pnl.Controls.Add(lbl3)

        txt3 = New TextBox
        txt3.Location = New Point(20, 195)
        txt3.Size = New Size(360, 25)
        txt3.Font = New Font("Arial", 10)
        pnl.Controls.Add(txt3)

        Dim lbl4 As New Label
        lbl4.Text = "Plaka:"
        lbl4.Location = New Point(20, 230)
        lbl4.Size = New Size(360, 20)
        lbl4.Font = New Font("Arial", 9, FontStyle.Bold)
        pnl.Controls.Add(lbl4)

        txt4 = New TextBox
        txt4.Location = New Point(20, 250)
        txt4.Size = New Size(360, 25)
        txt4.Font = New Font("Arial", 10)
        pnl.Controls.Add(txt4)

        Dim lbl5 As New Label
        lbl5.Text = "Kategori ID (1-5):"
        lbl5.Location = New Point(20, 285)
        lbl5.Size = New Size(360, 20)
        lbl5.Font = New Font("Arial", 9, FontStyle.Bold)
        pnl.Controls.Add(lbl5)

        txt5 = New TextBox
        txt5.Location = New Point(20, 305)
        txt5.Size = New Size(360, 25)
        txt5.Font = New Font("Arial", 10)
        pnl.Controls.Add(txt5)

        Dim lbl6 As New Label
        lbl6.Text = "Renk:"
        lbl6.Location = New Point(20, 340)
        lbl6.Size = New Size(360, 20)
        lbl6.Font = New Font("Arial", 9, FontStyle.Bold)
        pnl.Controls.Add(lbl6)

        txt6 = New TextBox
        txt6.Location = New Point(20, 360)
        txt6.Size = New Size(360, 25)
        txt6.Font = New Font("Arial", 10)
        pnl.Controls.Add(txt6)

        Dim btnSave As New Button
        btnSave.Text = "KAYDET"
        btnSave.Location = New Point(20, 395)
        btnSave.Size = New Size(170, 45)
        btnSave.BackColor = Color.LightGreen
        btnSave.Font = New Font("Arial", 11, FontStyle.Bold)
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        pnl.Controls.Add(btnSave)

        Dim btnCancel As New Button
        btnCancel.Text = "İPTAL"
        btnCancel.Location = New Point(210, 395)
        btnCancel.Size = New Size(170, 45)
        btnCancel.BackColor = Color.LightCoral
        btnCancel.Font = New Font("Arial", 11, FontStyle.Bold)
        AddHandler btnCancel.Click, AddressOf BtnCancel_Click
        pnl.Controls.Add(btnCancel)

        Me.Controls.Add(pnl)
    End Sub

    Private Sub CreateButton(parent As Panel, text As String, x As Integer, y As Integer, color As Color, handler As EventHandler)
        Dim btn As New Button
        btn.Text = text
        btn.Location = New Point(x, y)
        btn.Size = New Size(230, 40)
        btn.BackColor = color
        btn.ForeColor = Color.White
        btn.Font = New Font("Arial", 9, FontStyle.Bold)
        AddHandler btn.Click, handler
        parent.Controls.Add(btn)
    End Sub

    Private Sub TestDB()
        Try
            Using conn As New SqlConnection(connStr)
                conn.Open()
                lbl.Text = "  ✅ Veritabanı bağlantısı başarılı!"
                lbl.ForeColor = Color.Green
            End Using
        Catch ex As Exception
            lbl.Text = "  ❌ Bağlantı hatası: " & ex.Message
            lbl.ForeColor = Color.Red
        End Try
    End Sub

    Private Sub BtnCustomers_Click(sender As Object, e As EventArgs)
        pnl.Visible = False
        LoadData("SELECT CustomerID, FullName, Phone, Email, LicenseNumber, Address FROM Customers", "müşteri")
    End Sub

    Private Sub BtnAllVehicles_Click(sender As Object, e As EventArgs)
        pnl.Visible = False
        LoadData("SELECT VehicleID, Brand, Model, Year, Plate, CategoryID, Color, CASE WHEN IsAvailable=1 THEN 'Uygun' ELSE 'Kiralanmış' END AS Durum FROM Vehicles", "araç")
    End Sub

    Private Sub BtnAvailable_Click(sender As Object, e As EventArgs)
        pnl.Visible = False
        LoadData("SELECT VehicleID, Brand, Model, Year, Plate, Color FROM Vehicles WHERE IsAvailable = 1", "uygun araç")
    End Sub

    Private Sub BtnRentals_Click(sender As Object, e As EventArgs)
        pnl.Visible = False
        LoadData("SELECT r.RentalID, c.FullName AS Müşteri, v.Brand + ' ' + v.Model AS Araç, v.Plate AS Plaka, r.RentalDate AS [Kiralama Tarihi], r.PlannedReturnDate AS [Planlanan İade], r.ReturnDate AS [İade Tarihi], r.TotalPrice AS [Toplam Fiyat], r.Status AS Durum FROM Rentals r JOIN Customers c ON r.CustomerID = c.CustomerID JOIN Vehicles v ON r.VehicleID = v.VehicleID", "kiralama")
    End Sub

    Private Sub LoadData(query As String, dataType As String)
        Try
            Using conn As New SqlConnection(connStr)
                Dim dt As New DataTable()
                ' Eğer Visual Studio'nun eski bir sürümünü kullanıyorsanız, buradaki "New" kullanımı
                ' bir değişkene atanmazsa uyarı verebilir. Ancak modern VB.NET'te geçerlidir.
                ' Garanti olsun diye şöyle de kullanabilirsiniz:
                Dim da As New SqlDataAdapter(query, conn)
                da.Fill(dt)

                dgv.DataSource = dt
                lbl.Text = "  ✅ " & dt.Rows.Count.ToString() & " " & dataType & " listelendi"
                lbl.ForeColor = Color.Blue
            End Using
        Catch ex As Exception
            MessageBox.Show("Hata: " & ex.Message, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        isUpdateMode = False
        pnl.Visible = True
        pnl.BringToFront()
        txt1.Clear()
        txt2.Clear()
        txt3.Clear()
        txt4.Clear()
        txt5.Clear()
        txt6.Clear()
        txt1.Focus()

        Dim title = CType(pnl.Controls("lblTitle"), Label)
        title.Text = "YENİ ARAÇ EKLE"
        title.BackColor = Color.LightGreen

        lbl.Text = "  ➕ Yeni araç ekleniyor..."
        lbl.ForeColor = Color.Orange
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs)
        If dgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Lütfen güncellemek istediğiniz aracı seçin!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If dgv.Columns.Count < 7 Then
            MessageBox.Show("Önce 'Tüm Araçlar' butonuna basın!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim row = dgv.SelectedRows(0)
        updateVehicleId = Convert.ToInt32(row.Cells(0).Value)

        isUpdateMode = True
        pnl.Visible = True
        pnl.BringToFront()

        txt1.Text = row.Cells(1).Value.ToString()
        txt2.Text = row.Cells(2).Value.ToString()
        txt3.Text = row.Cells(3).Value.ToString()
        txt4.Text = row.Cells(4).Value.ToString()
        txt5.Text = row.Cells(5).Value.ToString()
        txt6.Text = row.Cells(6).Value.ToString()
        txt1.Focus()

        Dim title = CType(pnl.Controls("lblTitle"), Label)
        title.Text = "ARAÇ GÜNCELLE"
        title.BackColor = Color.LightBlue

        lbl.Text = "  🔄 Araç güncelleniyor..."
        lbl.ForeColor = Color.Orange
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
        If dgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Lütfen silmek istediğiniz aracı seçin!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If dgv.Columns.Count < 2 Then
            MessageBox.Show("Önce 'Tüm Araçlar' butonuna basın!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim row = dgv.SelectedRows(0)
        Dim vehicleId As Integer = Convert.ToInt32(row.Cells(0).Value)
        Dim vehicleName As String = row.Cells(1).Value.ToString() & " " & row.Cells(2).Value.ToString()

        Dim result = MessageBox.Show("'" & vehicleName & "' silinecek. Emin misiniz?", "ONAY", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.No Then
            Return
        End If

        Try
            Using conn As New SqlConnection(connStr)
                conn.Open()
                Using cmd As New SqlCommand("DELETE FROM Vehicles WHERE VehicleID = @id", conn)
                    cmd.Parameters.AddWithValue("@id", vehicleId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Araç silindi!", "BAŞARILI", MessageBoxButtons.OK, MessageBoxIcon.Information)
            lbl.Text = "  ✅ Araç silindi!"
            lbl.ForeColor = Color.Green
            BtnAllVehicles_Click(Nothing, Nothing)

        Catch ex As Exception
            MessageBox.Show("Silme hatası: " & ex.Message, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnRent_Click(sender As Object, e As EventArgs)
        If dgv.SelectedRows.Count = 0 Then
            MessageBox.Show("Lütfen kiralamak istediğiniz aracı seçin!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim row = dgv.SelectedRows(0)
        Dim vehicleId As Integer = Convert.ToInt32(row.Cells(0).Value)
        Dim vehicleName As String = row.Cells(1).Value.ToString() & " " & row.Cells(2).Value.ToString()
        Dim customerId As Integer = 1
        Dim days As Integer = 3
        Dim categoryId As Integer = 0
        Dim rentalPrice As Decimal = 0

        Dim result = MessageBox.Show("'" & vehicleName & "' aracını " & days.ToString() & " günlüğüne kiralamak istiyor musunuz?", "ONAY", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.No Then
            Return
        End If

        Try
            Using conn As New SqlConnection(connStr)
                conn.Open()

                Using cmd As New SqlCommand("SELECT CategoryID FROM Vehicles WHERE VehicleID = @id", conn)
                    cmd.Parameters.AddWithValue("@id", vehicleId)
                    ' Tip dönüşümleri için güvenlik kontrolü eklendi
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                        categoryId = Convert.ToInt32(res)
                    End If
                End Using

                Using cmd As New SqlCommand("SELECT DailyPrice FROM Categories WHERE CategoryID = @id", conn)
                    cmd.Parameters.AddWithValue("@id", categoryId)
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                        rentalPrice = Convert.ToDecimal(res)
                    End If
                End Using

                Using cmd As New SqlCommand("INSERT INTO Rentals (VehicleID, CustomerID, PlannedReturnDate, DailyPrice, Status) VALUES (@vid, @cid, @return, @price, 'Active')", conn)
                    cmd.Parameters.AddWithValue("@vid", vehicleId)
                    cmd.Parameters.AddWithValue("@cid", customerId)
                    cmd.Parameters.AddWithValue("@return", DateTime.Now.AddDays(days))
                    cmd.Parameters.AddWithValue("@price", rentalPrice)
                    cmd.ExecuteNonQuery()
                End Using

                Using cmd As New SqlCommand("UPDATE Vehicles SET IsAvailable = 0 WHERE VehicleID = @id", conn)
                    cmd.Parameters.AddWithValue("@id", vehicleId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Araç kiralandı! Günlük fiyat: " & rentalPrice.ToString() & " TL", "BAŞARILI", MessageBoxButtons.OK, MessageBoxIcon.Information)
            lbl.Text = "  ✅ Araç kiralandı!"
            lbl.ForeColor = Color.Green
            BtnAvailable_Click(Nothing, Nothing)

        Catch ex As Exception
            MessageBox.Show("Kiralama hatası: " & ex.Message, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txt1.Text) Or String.IsNullOrWhiteSpace(txt2.Text) Or String.IsNullOrWhiteSpace(txt3.Text) Or String.IsNullOrWhiteSpace(txt4.Text) Or String.IsNullOrWhiteSpace(txt5.Text) Then
            MessageBox.Show("Lütfen zorunlu alanları doldurun!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Using conn As New SqlConnection(connStr)
                conn.Open()

                If isUpdateMode Then
                    Dim colorValue As Object = If(String.IsNullOrWhiteSpace(txt6.Text), DBNull.Value, CObj(txt6.Text))
                    Using cmd As New SqlCommand("UPDATE Vehicles SET Brand=@b, Model=@m, Year=@y, Plate=@p, CategoryID=@c, Color=@col WHERE VehicleID=@id", conn)
                        cmd.Parameters.AddWithValue("@b", txt1.Text)
                        cmd.Parameters.AddWithValue("@m", txt2.Text)
                        cmd.Parameters.AddWithValue("@y", Integer.Parse(txt3.Text))
                        cmd.Parameters.AddWithValue("@p", txt4.Text)
                        cmd.Parameters.AddWithValue("@c", Integer.Parse(txt5.Text))
                        cmd.Parameters.AddWithValue("@col", colorValue)
                        cmd.Parameters.AddWithValue("@id", updateVehicleId)
                        cmd.ExecuteNonQuery()
                    End Using
                    MessageBox.Show("Araç güncellendi!", "BAŞARILI", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    lbl.Text = "  ✅ Araç güncellendi!"
                Else
                    Dim colorValue As Object = If(String.IsNullOrWhiteSpace(txt6.Text), DBNull.Value, CObj(txt6.Text))
                    Using cmd As New SqlCommand("INSERT INTO Vehicles (Brand, Model, Year, Plate, CategoryID, Color, IsAvailable) VALUES (@b, @m, @y, @p, @c, @col, 1)", conn)
                        cmd.Parameters.AddWithValue("@b", txt1.Text)
                        cmd.Parameters.AddWithValue("@m", txt2.Text)
                        cmd.Parameters.AddWithValue("@y", Integer.Parse(txt3.Text))
                        cmd.Parameters.AddWithValue("@p", txt4.Text)
                        cmd.Parameters.AddWithValue("@c", Integer.Parse(txt5.Text))
                        cmd.Parameters.AddWithValue("@col", colorValue)
                        cmd.ExecuteNonQuery()
                    End Using
                    MessageBox.Show("Araç eklendi!", "BAŞARILI", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    lbl.Text = "  ✅ Yeni araç eklendi!"
                End If
            End Using

            pnl.Visible = False
            lbl.ForeColor = Color.Green
            BtnAllVehicles_Click(Nothing, Nothing)

        Catch ex As Exception
            MessageBox.Show("Kayıt hatası: " & ex.Message, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs)
        pnl.Visible = False
        lbl.Text = "  ❌ İşlem iptal edildi"
        lbl.ForeColor = Color.Gray
    End Sub
End Class