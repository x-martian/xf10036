Imports System.Reflection

Class ApplicationMainWindow

#Region " Constructor "

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        EventManager.RegisterClassHandler(GetType(UserControlBase), UserControlBase.CloseParentContainerRoutedEvent, New RoutedEventHandler(AddressOf CloseParentContainerHandler))

    End Sub

#End Region

#Region " Methods "

    Private Sub ApplicationMainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        LoadMenu()

    End Sub

    Private Sub ApplicationMainWindow_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.PreviewKeyDown

        If Me.tcOpenForms.Items.Count > 0 AndAlso Me.ucTaskSwitcherControl.Visibility = Windows.Visibility.Collapsed AndAlso e.Key = Key.Tab AndAlso e.KeyboardDevice.Modifiers = ModifierKeys.Control Then
            Me.ucTaskSwitcherControl.Visibility = Windows.Visibility.Visible
            Me.ucTaskSwitcherControl.Show(Me.tcOpenForms)
            e.Handled = True
        End If

    End Sub

    Private Sub ApplicationMainWindow_PreviewKeyUp(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.PreviewKeyUp

        If Me.ucTaskSwitcherControl.Visibility = Windows.Visibility.Visible AndAlso e.KeyboardDevice.Modifiers <> ModifierKeys.Control Then
            Me.ucTaskSwitcherControl.Hide()
        End If

    End Sub

    Private Sub CloseParentContainerHandler(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.tcOpenForms.Items.Remove(CType(CType(e.Source, UserControlBase).Parent, TabItem))

    End Sub

    Private Function GetMatchingForm(ByVal strIn As String) As Integer

        Dim intHighestInstance As Integer = -1

        For Each obj As TabItem In Me.tcOpenForms.Items

            Dim objTabItemMetaData As TabItemMetaData = CType(obj.Tag, TabItemMetaData)

            If objTabItemMetaData.TabHeader = strIn Then

                If objTabItemMetaData.InstanceCount > intHighestInstance Then
                    intHighestInstance = objTabItemMetaData.InstanceCount
                End If

            End If

        Next

        Return intHighestInstance

    End Function

    Private Sub LoadMenu()

        'this normally comes from a database, based on the user security context
        Dim objTabs As New List(Of TabItemMetaData)
        objTabs.Add(New TabItemMetaData("Pet Management : Pet Maintenance", "Pet Maint", "MDITaskSwitchingSample.PetMaintenanceControl"))
        objTabs.Add(New TabItemMetaData("Pet Management : Pet Inventory", "Pet Inventory", "MDITaskSwitchingSample.PetInventoryControl"))
        objTabs.Add(New TabItemMetaData("Pet Management : Pet Homes", "Pet Homes", "MDITaskSwitchingSample.PetHomesControl"))

        Dim objPetMenu As New MenuItem
        objPetMenu.Header = "_Pet"

        For Each obj As TabItemMetaData In objTabs

            Dim objPetMenuItem As New MenuItem
            objPetMenuItem.Header = obj.TabHeader
            objPetMenuItem.CommandParameter = obj
            objPetMenuItem.AddHandler(MenuItem.ClickEvent, New RoutedEventHandler(AddressOf OnPetMenuClick))
            objPetMenu.Items.Add(objPetMenuItem)
        Next

        Me.mnuApplicationMenu.Items.Add(objPetMenu)

    End Sub

    Private Sub OnMenuItemExitClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()

    End Sub

    Private Sub OnPetMenuClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

        Dim objTabItemMetaData As TabItemMetaData = DirectCast(DirectCast(e.Source, System.Windows.Controls.MenuItem).CommandParameter, MDITaskSwitchingSample.TabItemMetaData).Clone
        objTabItemMetaData.InstanceCount = GetMatchingForm(objTabItemMetaData.TabHeader) + 1

        Dim asm As Assembly = Assembly.GetExecutingAssembly
        Dim objFormControl As UserControl = asm.CreateInstance(objTabItemMetaData.UserControlFormName, True)

        Dim objTabItem As New TabItem

        If objTabItemMetaData.InstanceCount > 0 Then
            objTabItem.Header = String.Format("{0}  ({1})", objTabItemMetaData.TabHeader, objTabItemMetaData.InstanceCount.ToString)

        Else
            objTabItem.Header = objTabItemMetaData.TabHeader
        End If

        objTabItem.Tag = objTabItemMetaData
        objTabItem.Content = objFormControl
        Me.tcOpenForms.SelectedIndex = Me.tcOpenForms.Items.Add(objTabItem)

    End Sub

#End Region

End Class
