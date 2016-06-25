'MIT License
'
'Copyright (c) 2016 [ihmyw00l711]
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in all
'copies or substantial portions of the Software.

Imports System.Collections
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace MobileDeviceVB


	Public Class iPhone
		Private connected As Boolean
		Private current_directory As String
		Friend DFUHandle As AMRecoveryDevice
		Private dnc As DeviceNotificationCallback
		Private drn1 As DeviceRestoreNotificationCallback
		Private drn2 As DeviceRestoreNotificationCallback
		Private drn3 As DeviceRestoreNotificationCallback
		Private drn4 As DeviceRestoreNotificationCallback
		Friend hAFC As Pointer(Of System.Void)
		Friend hService As Pointer(Of System.Void)
		Friend iPhoneHandle As Pointer(Of System.Void)
		Private Shared path_separators As Char() = New Char() {"/"C}
		Private wasAFC2 As Boolean

		Public Event Connect As ConnectEventHandler

		Public Event DfuConnect As DeviceNotificationEventHandler

		Public Event DfuDisconnect As DeviceNotificationEventHandler

		Public Event Disconnect As ConnectEventHandler

		Public Event RecoveryModeEnter As DeviceNotificationEventHandler

		Public Event RecoveryModeLeave As DeviceNotificationEventHandler

		Public Sub New()
			Me.wasAFC2 = False
			Me.doConstruction()
		End Sub

		Public Sub New(myConnectHandler As ConnectEventHandler, myDisconnectHandler As ConnectEventHandler)
			Me.wasAFC2 = False
			Me.Connect = DirectCast([Delegate].Combine(Me.Connect, myConnectHandler), ConnectEventHandler)
			Me.Disconnect = DirectCast([Delegate].Combine(Me.Disconnect, myDisconnectHandler), ConnectEventHandler)
			Me.doConstruction()
		End Sub

		Private Function ConnectToPhone() As Boolean
			If MobileDevice.AMDeviceConnect(Me.iPhoneHandle) = 1 Then
				Throw New Exception("Phone in recovery mode, support not yet implemented")
			End If
			If MobileDevice.AMDeviceIsPaired(Me.iPhoneHandle) = 0 Then
				Return False
			End If
			If MobileDevice.AMDeviceValidatePairing(Me.iPhoneHandle) <> 0 Then
				Return False
			End If
			If MobileDevice.AMDeviceStartSession(Me.iPhoneHandle) = 1 Then
				Return False
			End If
			If MobileDevice.AMDeviceStartService(Me.iPhoneHandle, MobileDevice.CFStringMakeConstantString("com.apple.afc2"), Me.hService, Nothing) <> 0 Then
				If MobileDevice.AMDeviceStartService(Me.iPhoneHandle, MobileDevice.CFStringMakeConstantString("com.apple.afc"), Me.hService, Nothing) <> 0 Then
					Return False
				End If
			Else
				Me.wasAFC2 = True
			End If
			If MobileDevice.AFCConnectionOpen(Me.hService, 0, Me.hAFC) <> 0 Then
				Return False
			End If
			Me.connected = True
			Return True
		End Function

		Public Sub Copy(sourceName As String, destName As String)
		End Sub

		Public Function CreateDirectory(path As String) As Boolean
			Return (MobileDevice.AFCDirectoryCreate(Me.hAFC, Me.FullPath(Me.CurrentDirectory, path)) = 0)
		End Function

		Public Sub DeleteDirectory(path As String)
			Dim str As String = Me.FullPath(Me.CurrentDirectory, path)
			If Me.IsDirectory(str) Then
				MobileDevice.AFCRemovePath(Me.hAFC, str)
			End If
		End Sub

		Public Sub DeleteDirectory(path As String, recursive As Boolean)
			If Not recursive Then
				Me.DeleteDirectory(path)
			Else
				Dim str As String = Me.FullPath(Me.CurrentDirectory, path)
				If Me.IsDirectory(str) Then
					Me.InternalDeleteDirectory(path)
				End If
			End If
		End Sub

		Public Sub DeleteFile(path As String)
			Dim str As String = Me.FullPath(Me.CurrentDirectory, path)
			If Me.Exists(str) Then
				MobileDevice.AFCRemovePath(Me.hAFC, str)
			End If
		End Sub

		Private Sub DfuConnectCallback(ByRef callback As AMRecoveryDevice)
			Me.DFUHandle = callback
			Me.OnDfuConnect(New DeviceNotificationEventArgs(callback))
		End Sub

		Private Sub DfuDisconnectCallback(ByRef callback As AMRecoveryDevice)
			Me.DFUHandle = callback
			Me.OnDfuDisconnect(New DeviceNotificationEventArgs(callback))
		End Sub

		Private Sub doConstruction()
			Dim voidPtr As Pointer(Of System.Void)
			Me.dnc = New DeviceNotificationCallback(AddressOf Me.NotifyCallback)
			Me.drn1 = New DeviceRestoreNotificationCallback(AddressOf Me.DfuConnectCallback)
			Me.drn2 = New DeviceRestoreNotificationCallback(AddressOf Me.RecoveryConnectCallback)
			Me.drn3 = New DeviceRestoreNotificationCallback(AddressOf Me.DfuDisconnectCallback)
			Me.drn4 = New DeviceRestoreNotificationCallback(AddressOf Me.RecoveryDisconnectCallback)
			Dim num As Integer = MobileDevice.AMDeviceNotificationSubscribe(Me.dnc, 0, 0, 0, voidPtr)
			If num <> 0 Then
				Throw New Exception("AMDeviceNotificationSubscribe failed with error " + num)
			End If
			num = MobileDevice.AMRestoreRegisterForDeviceNotifications(Me.drn1, Me.drn2, Me.drn3, Me.drn4, 0, Nothing)
			If num <> 0 Then
				Throw New Exception("AMRestoreRegisterForDeviceNotifications failed with error " + num)
			End If
			Me.current_directory = "/"
		End Sub

		Public Function Exists(path As String) As Boolean
			Dim dict As Pointer(Of System.Void) = Nothing
			Dim num As Integer = MobileDevice.AFCFileInfoOpen(Me.hAFC, path, dict)
			If num = 0 Then
				MobileDevice.AFCKeyValueClose(dict)
			End If
			Return (num = 0)
		End Function

		Public Function FileSize(path As String) As ULong
			Dim flag As Boolean
			Dim num As ULong
			Me.GetFileInfo(path, num, flag)
			Return num
		End Function

		Public Function FSBlockSize() As Integer
			Return MobileDevice.AFCConnectionGetFSBlockSize(Me.hAFC)
		End Function

		Friend Function FullPath(path1 As String, path2 As String) As String
			Dim strArray As String()
			If (path1 Is Nothing) OrElse (path1 = String.Empty) Then
				path1 = "/"
			End If
			If (path2 Is Nothing) OrElse (path2 = String.Empty) Then
				path2 = "/"
			End If
			If path2(0) = "/"C Then
				strArray = path2.Split(path_separators)
			ElseIf path1(0) = "/"C Then
				strArray = (Convert.ToString(path1 & Convert.ToString("/")) & path2).Split(path_separators)
			Else
				strArray = (Convert.ToString((Convert.ToString("/") & path1) + "/") & path2).Split(path_separators)
			End If
			Dim strArray2 As String() = New String(strArray.Length - 1) {}
			Dim count As Integer = 0
			For i As Integer = 0 To strArray.Length - 1
				If strArray(i) = ".." Then
					If count > 0 Then
						count -= 1
					End If
				ElseIf (strArray(i) <> ".") AndAlso Not (strArray(i) = "") Then
					strArray2(System.Math.Max(System.Threading.Interlocked.Increment(count),count - 1)) = strArray(i)
				End If
			Next
			Return ("/" + String.Join("/", strArray2, 0, count))
		End Function

		Private Function Get_st_ifmt(path As String) As String
			Return Me.GetFileInfo(path)("st_ifmt")
		End Function

		Public Function GetCopyDeviceIdentifier() As String
			Return Marshal.PtrToStringAnsi(MobileDevice.AMDeviceCopyDeviceIdentifier(Me.iPhoneHandle))
		End Function

		Public Function GetDeviceInfo() As Dictionary(Of String, String)
			Dim dictionary As New Dictionary(Of String, String)()
			Dim dict As Pointer(Of System.Void) = Nothing
			If (MobileDevice.AFCDeviceInfoOpen(Me.hAFC, dict) = 0) AndAlso (dict IsNot Nothing) Then
				Dim voidPtr2 As Pointer(Of System.Void)
				Dim voidPtr3 As Pointer(Of System.Void)
				While ((MobileDevice.AFCKeyValueRead(dict, voidPtr2, voidPtr3) = 0) AndAlso (voidPtr2 IsNot Nothing)) AndAlso (voidPtr3 IsNot Nothing)
					Dim key As String = Marshal.PtrToStringAnsi(New IntPtr(voidPtr2))
					Dim str2 As String = Marshal.PtrToStringAnsi(New IntPtr(voidPtr3))
					dictionary.Add(key, str2)
				End While
				MobileDevice.AFCKeyValueClose(dict)
			End If
			Return dictionary
		End Function

		Public Function GetDFUImei() As String
			Try
				Return MobileDevice.AMRecoveryModeDeviceCopySerialNumber(Me.DFUHandle)
			Catch
				Return "NO"
			End Try
		End Function

		Public Function GetDirectories(path As String) As String()
			If Not Me.IsConnected Then
				Throw New Exception("Not connected to phone")
			End If
			Dim dir As Pointer(Of System.Void) = Nothing
			Dim str As String = Me.FullPath(Me.CurrentDirectory, path)
			Dim num As Integer = MobileDevice.AFCDirectoryOpen(Me.hAFC, str, dir)
			If num <> 0 Then
				Throw New Exception("Path does not exist: " + num.ToString())
			End If
			Dim buffer As String = Nothing
			Dim list As New ArrayList()
			MobileDevice.AFCDirectoryRead(Me.hAFC, dir, buffer)
			While buffer IsNot Nothing
				If ((buffer <> ".") AndAlso (buffer <> "..")) AndAlso Me.IsDirectory(Me.FullPath(str, buffer)) Then
					list.Add(buffer)
				End If
				MobileDevice.AFCDirectoryRead(Me.hAFC, dir, buffer)
			End While
			MobileDevice.AFCDirectoryClose(Me.hAFC, dir)
			Return DirectCast(list.ToArray(GetType(String)), String())
		End Function

		Public Function GetDirectoryRoot(path As String) As String
			Return "/"
		End Function

		Public Function GetFileInfo(path As String) As Dictionary(Of String, String)
			Dim dictionary As New Dictionary(Of String, String)()
			Dim dict As Pointer(Of System.Void) = Nothing
			If (MobileDevice.AFCFileInfoOpen(Me.hAFC, path, dict) = 0) AndAlso (dict IsNot Nothing) Then
				Dim voidPtr2 As Pointer(Of System.Void)
				Dim voidPtr3 As Pointer(Of System.Void)
				While ((MobileDevice.AFCKeyValueRead(dict, voidPtr2, voidPtr3) = 0) AndAlso (voidPtr2 IsNot Nothing)) AndAlso (voidPtr3 IsNot Nothing)
					Dim key As String = Marshal.PtrToStringAnsi(New IntPtr(voidPtr2))
					Dim str2 As String = Marshal.PtrToStringAnsi(New IntPtr(voidPtr3))
					dictionary.Add(key, str2)
				End While
				MobileDevice.AFCKeyValueClose(dict)
			End If
			Return dictionary
		End Function

		Public Sub GetFileInfo(path As String, ByRef size As ULong, ByRef directory As Boolean)
			Dim fileInfo As Dictionary(Of String, String) = Me.GetFileInfo(path)
			size = If(fileInfo.ContainsKey("st_size"), ULong.Parse(fileInfo("st_size")), CULng(0L))
			Dim flag As Boolean = False
			directory = False
			If fileInfo.ContainsKey("st_ifmt") Then
				Dim str As String = fileInfo("st_ifmt")
				If str IsNot Nothing Then
					If Not (str = "S_IFDIR") Then
						If str = "S_IFLNK" Then
							flag = True
						End If
					Else
						directory = True
					End If
				End If
			End If
			If flag Then
				Dim flag3 As Boolean
				Dim dir As Pointer(Of System.Void) = Nothing
				directory = InlineAssignHelper(flag3, MobileDevice.AFCDirectoryOpen(Me.hAFC, path, dir) = 0)
				If flag3 Then
					MobileDevice.AFCDirectoryClose(Me.hAFC, dir)
				End If
			End If
		End Sub

		Public Function GetFiles(path As String) As String()
			If Not Me.IsConnected Then
				Throw New Exception("Not connected to phone")
			End If
			Dim str As String = Me.FullPath(Me.CurrentDirectory, path)
			Dim dir As Pointer(Of System.Void) = Nothing
			If MobileDevice.AFCDirectoryOpen(Me.hAFC, str, dir) <> 0 Then
				Throw New Exception("Path does not exist")
			End If
			Dim buffer As String = Nothing
			Dim list As New ArrayList()
			MobileDevice.AFCDirectoryRead(Me.hAFC, dir, buffer)
			While buffer IsNot Nothing
				If Not Me.IsDirectory(Me.FullPath(str, buffer)) Then
					list.Add(buffer)
				End If
				MobileDevice.AFCDirectoryRead(Me.hAFC, dir, buffer)
			End While
			MobileDevice.AFCDirectoryClose(Me.hAFC, dir)
			Return DirectCast(list.ToArray(GetType(String)), String())
		End Function

		Public Function GetiphoneSize() As Integer
			Dim dict As Pointer(Of System.Void) = Nothing
			Dim num As Integer = 0
			If (MobileDevice.AFCDeviceInfoOpen(Me.hAFC, dict) = 0) AndAlso (dict IsNot Nothing) Then
				num = CType(dict, Pointer(Of Integer)).Target
			End If
			Return num
		End Function

		Public Function GetiPhoneStr(str As String) As String
			Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, str)
		End Function

		Public Function GoOutDFU() As Integer
			Dim num As Integer = &Hff
			Try
				num = MobileDevice.AMRecoveryModeDeviceSetAutoBoot(Me.DFUHandle, 1)
				Return MobileDevice.AMRecoveryModeDeviceReboot(Me.DFUHandle)
			Catch
				Return num
			End Try
		End Function

		Public Function GotoDFU() As Integer
			Return MobileDevice.AMDeviceEnterRecovery(Me.iPhoneHandle)
		End Function

		Private Sub InternalDeleteDirectory(path As String)
			Dim num As Integer
			Dim str As String = Me.FullPath(Me.CurrentDirectory, path)
			Dim files As String() = Me.GetFiles(path)
			For num = 0 To files.Length - 1
				Me.DeleteFile((str & Convert.ToString("/")) + files(num))
			Next
			files = Me.GetDirectories(path)
			For num = 0 To files.Length - 1
				Me.InternalDeleteDirectory((str & Convert.ToString("/")) + files(num))
			Next
			Me.DeleteDirectory(path)
		End Sub

		Public Function IsDirectory(path As String) As Boolean
			Dim flag As Boolean
			Dim num As ULong
			Me.GetFileInfo(path, num, flag)
			Return flag
		End Function

		Public Function IsFile(path As String) As Boolean
			Return (Me.Get_st_ifmt(path) = "S_IFREG")
		End Function

		Public Function IsLink(path As String) As Boolean
			Return (Me.Get_st_ifmt(path) = "S_IFLNK")
		End Function

		Private Sub NotifyCallback(ByRef callback As AMDeviceNotificationCallbackInfo)
			If callback.msg = NotificationMessage.Connected Then
				Me.iPhoneHandle = callback.dev
				If Me.ConnectToPhone() Then
					Me.OnConnect(New ConnectEventArgs(callback))
				End If
			ElseIf callback.msg = NotificationMessage.Disconnected Then
				Me.connected = False
				Me.OnDisconnect(New ConnectEventArgs(callback))
			End If
		End Sub

		Protected Sub OnConnect(args As ConnectEventArgs)
			Dim connect As ConnectEventHandler = Me.Connect
			RaiseEvent connect(Me, args)
		End Sub

		Protected Sub OnDfuConnect(args As DeviceNotificationEventArgs)
			Dim dfuConnect As DeviceNotificationEventHandler = Me.DfuConnect
			RaiseEvent dfuConnect(Me, args)
		End Sub

		Protected Sub OnDfuDisconnect(args As DeviceNotificationEventArgs)
			Dim dfuDisconnect As DeviceNotificationEventHandler = Me.DfuDisconnect
			RaiseEvent dfuDisconnect(Me, args)
		End Sub

		Protected Sub OnDisconnect(args As ConnectEventArgs)
			Dim disconnect As ConnectEventHandler = Me.Disconnect
			RaiseEvent disconnect(Me, args)
		End Sub

		Protected Sub OnRecoveryModeEnter(args As DeviceNotificationEventArgs)
			Dim recoveryModeEnter As DeviceNotificationEventHandler = Me.RecoveryModeEnter
			RaiseEvent recoveryModeEnter(Me, args)
		End Sub

		Protected Sub OnRecoveryModeLeave(args As DeviceNotificationEventArgs)
			Dim recoveryModeLeave As DeviceNotificationEventHandler = Me.RecoveryModeLeave
			RaiseEvent recoveryModeLeave(Me, args)
		End Sub

		Public Sub ReConnect()
			Dim num As Integer = MobileDevice.AFCConnectionClose(Me.hAFC)
			num = MobileDevice.AMDeviceStopSession(Me.iPhoneHandle)
			num = MobileDevice.AMDeviceDisconnect(Me.iPhoneHandle)
			Me.ConnectToPhone()
		End Sub

		Private Sub RecoveryConnectCallback(ByRef callback As AMRecoveryDevice)
			Me.DFUHandle = callback
			Me.OnRecoveryModeEnter(New DeviceNotificationEventArgs(callback))
		End Sub

		Private Sub RecoveryDisconnectCallback(ByRef callback As AMRecoveryDevice)
			Me.DFUHandle = callback
			Me.OnRecoveryModeLeave(New DeviceNotificationEventArgs(callback))
		End Sub

		Public Function Rename(sourceName As String, destName As String) As Boolean
			Return (MobileDevice.AFCRenamePath(Me.hAFC, Me.FullPath(Me.CurrentDirectory, sourceName), Me.FullPath(Me.CurrentDirectory, destName)) = 0)
		End Function

		Public ReadOnly Property AFCHandle() As Pointer(Of System.Void)
			Get
				Return Me.hAFC
			End Get
		End Property

		Public Property CurrentDirectory() As String
			Get
				Return Me.current_directory
			End Get
			Set
				Dim path As String = Me.FullPath(Me.current_directory, value)
				If Not Me.IsDirectory(path) Then
					Throw New Exception("Invalid directory specified")
				End If
				Me.current_directory = path
			End Set
		End Property

		Public ReadOnly Property Device() As Pointer(Of System.Void)
			Get
				Return Me.iPhoneHandle
			End Get
		End Property

		Public ReadOnly Property GetActivationState() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "ActivationState")
			End Get
		End Property

		Public ReadOnly Property GetBasebandBootloaderVersion() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "BasebandBootloaderVersion")
			End Get
		End Property

		Public ReadOnly Property GetBasebandSerialNumber() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "BasebandSerialNumber")
			End Get
		End Property

		Public ReadOnly Property GetBasebandVersion() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "BasebandVersion")
			End Get
		End Property

		Public ReadOnly Property GetBluetoothAddress() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "BluetoothAddress")
			End Get
		End Property

		Public ReadOnly Property GetBuildVersion() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "BuildVersion")
			End Get
		End Property

		Public ReadOnly Property GetCPUArchitecture() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "CPUArchitecture")
			End Get
		End Property

		Public ReadOnly Property GetDeviceCertificate() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "DeviceCertificate")
			End Get
		End Property

		Public ReadOnly Property GetDeviceClass() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "DeviceClass")
			End Get
		End Property

		Public ReadOnly Property GetDeviceColor() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "DeviceColor")
			End Get
		End Property

		Public ReadOnly Property GetDeviceName() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "DeviceName")
			End Get
		End Property

		Public ReadOnly Property GetDevicePublicKey() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "DevicePublicKey")
			End Get
		End Property

		Public ReadOnly Property GetDeviceVersion() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "ProductVersion")
			End Get
		End Property

		Public ReadOnly Property GetDieID() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "DieID")
			End Get
		End Property

		Public ReadOnly Property GetFirmwareVersion() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "FirmwareVersion")
			End Get
		End Property

		Public ReadOnly Property GetHardwareModel() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "HardwareModel")
			End Get
		End Property

		Public ReadOnly Property GetHostAttached() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "HostAttached")
			End Get
		End Property

		Public ReadOnly Property GetIntegratedCircuitCardIdentity() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "IntegratedCircuitCardIdentity")
			End Get
		End Property

		Public ReadOnly Property GetInternationalMobileEquipmentIdentity() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "InternationalMobileEquipmentIdentity")
			End Get
		End Property

		Public ReadOnly Property GetInternationalMobileSubscriberIdentity() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "InternationalMobileSubscriberIdentity")
			End Get
		End Property

		Public ReadOnly Property GetiTunesHasConnected() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "iTunesHasConnected")
			End Get
		End Property

		Public ReadOnly Property GetMLBSerialNumber() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "MLBSerialNumber")
			End Get
		End Property

		Public ReadOnly Property GetMobileSubscriberCountryCode() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "MobileSubscriberCountryCode")
			End Get
		End Property

		Public ReadOnly Property GetMobileSubscriberNetworkCode() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "MobileSubscriberNetworkCode")
			End Get
		End Property

		Public ReadOnly Property GetModelNumber() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "ModelNumber")
			End Get
		End Property

		Public ReadOnly Property GetPasswordProtected() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "PasswordProtected")
			End Get
		End Property

		Public ReadOnly Property GetPhoneNumber() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "PhoneNumber")
			End Get
		End Property

		Public ReadOnly Property GetProductionSOC() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "ProductionSOC")
			End Get
		End Property

		Public ReadOnly Property GetProductType() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "ProductType")
			End Get
		End Property

		Public ReadOnly Property GetProtocolVersion() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "ProtocolVersion")
			End Get
		End Property

		Public ReadOnly Property GetRegionInfo() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "RegionInfo")
			End Get
		End Property

		Public ReadOnly Property GetSBLockdownEverRegisteredKey() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "SBLockdownEverRegisteredKey")
			End Get
		End Property

		Public ReadOnly Property GetSerialNumber() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "SerialNumber")
			End Get
		End Property

		Public ReadOnly Property GetSIMStatus() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "SIMStatus")
			End Get
		End Property

		Public ReadOnly Property GetUniqueChipID() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "UniqueChipID")
			End Get
		End Property

		Public ReadOnly Property GetUniqueDeviceID() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "UniqueDeviceID")
			End Get
		End Property

		Public ReadOnly Property GetWiFiAddress() As String
			Get
				Return MobileDevice.AMDeviceCopyValue(Me.iPhoneHandle, "WiFiAddress")
			End Get
		End Property

		Public ReadOnly Property IsConnected() As Boolean
			Get
				Return Me.connected
			End Get
		End Property

		Public ReadOnly Property IsJailbreak() As Boolean
			Get
				Return (Me.wasAFC2 OrElse (If(Me.connected, Me.Exists("/Applications"), False)))
			End Get
		End Property

		<StructLayout(LayoutKind.Sequential)> _
		Public Structure strFileInfo
			Public name As Byte()
			Public size As ULong
			Public isDir As Boolean
			Public isSLink As Boolean
		End Structure

		<StructLayout(LayoutKind.Sequential)> _
		Public Structure strStatVfs
			Public Namemax As Integer
			Public Bsize As Integer
			Public Btotal As Single
			Public Bfree As Single
		End Structure
		Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
			target = value
			Return value
		End Function
	End Class
End Namespace

