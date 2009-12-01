Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
'
<Serializable()> _
Public Class TabItemMetaData
    Implements ICloneable

#Region " Declarations "

    Private _strApplicationName As String = String.Empty
    Private _strTabHeader As String = String.Empty
    Private _strUserControlFormName As String = String.Empty
    Private _intTabControlIndex As Integer
    Private _intInstanceCount As Integer

#End Region

#Region " Properties "

    Public Property InstanceCount() As Integer
        Get
            Return _intInstanceCount
        End Get
        Set(ByVal Value As Integer)
            _intInstanceCount = Value
        End Set
    End Property

    Public Property TabControlIndex() As Integer
        Get
            Return _intTabControlIndex
        End Get
        Set(ByVal Value As Integer)
            _intTabControlIndex = Value
        End Set
    End Property

    Public ReadOnly Property ApplicationName() As String
        Get
            Return _strApplicationName
        End Get
    End Property

    Public ReadOnly Property TabHeader() As String
        Get
            Return _strTabHeader
        End Get
    End Property

    Public ReadOnly Property UserControlFormName() As String
        Get
            Return _strUserControlFormName
        End Get
    End Property

#End Region

#Region " Constructor "

    Public Sub New(ByVal strApplicationName As String, ByVal strTabHeader As String, ByVal strUserControlFormName As String)
        _strApplicationName = strApplicationName
        _strTabHeader = strTabHeader
        _strUserControlFormName = strUserControlFormName

    End Sub

#End Region

#Region " Events "

    'this is a little hack.  it allows callers to call this
    'method directly and get back a strongly typed object
    Public Function Clone() As TabItemMetaData
        Return DirectCast(Clone1(), TabItemMetaData)

    End Function

    'this is a little hack.  it allows callers to call this
    'method using the Interface.
    Private Function Clone1() As Object Implements System.ICloneable.Clone

        Dim obj As Object
        Using ms As MemoryStream = New MemoryStream

            Dim bf As BinaryFormatter = New BinaryFormatter
            bf.Serialize(ms, Me)
            ms.Position = 0
            obj = bf.Deserialize(ms)
        End Using
        Return obj

    End Function

#End Region

End Class
