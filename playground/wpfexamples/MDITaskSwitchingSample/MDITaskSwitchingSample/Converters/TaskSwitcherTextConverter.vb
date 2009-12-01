'
<ValueConversion(GetType(TabItemMetaData), GetType(String))> _
Public Class TaskSwitcherTextConverter
    Implements IValueConverter

#Region " Methods "

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert

        Dim obj As TabItemMetaData = DirectCast(value, MDITaskSwitchingSample.TabItemMetaData)

        If obj.InstanceCount = 0 Then
            Return obj.TabHeader

        Else
            Return String.Format("{0}  ({1})", obj.TabHeader, obj.InstanceCount.ToString)
        End If

    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException

    End Function

#End Region

End Class
