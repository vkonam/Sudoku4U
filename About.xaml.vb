Imports Microsoft.Phone.Tasks

Partial Public Class About
    Inherits PhoneApplicationPage


    Dim wb As New WebBrowser

    Public Sub New()
        InitializeComponent()
    End Sub


    Private Sub btnRate_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnRate.Click
        'MessageBox.Show("Hi")
        'wb.Navigate(New Uri("http://windowsphone.com/s?appid=84483606-748f-468b-ae36-c79524f45992", UriKind.Absolute))
        Dim marketplaceReviewTask As MarketplaceReviewTask = New MarketplaceReviewTask()
        marketplaceReviewTask.Show()

    End Sub
End Class
