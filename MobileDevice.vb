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

Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports Microsoft.Win32
Namespace MobileDeviceVB

	Friend Class MobileDevice
		Private Shared ReadOnly ApplicationSupportDirectory As New DirectoryInfo(Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Apple Inc.\Apple Application Support", "InstallDir", Environment.CurrentDirectory).ToString())
		Private Const DLLName As String = "iTunesMobileDevice.dll"
		Private Shared ReadOnly iTunesMobileDeviceFile As New FileInfo(Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Apple Inc.\Apple Mobile Device Support\Shared", "iTunesMobileDeviceDLL", "iTunesMobileDevice.dll").ToString())

		Shared Sub New()
			Dim directoryName As String = iTunesMobileDeviceFile.DirectoryName
			If Not iTunesMobileDeviceFile.Exists Then
				directoryName = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\Apple\Mobile Device Support\bin"
				If Not File.Exists(directoryName & Convert.ToString("\iTunesMobileDevice.dll")) Then
					directoryName = "C:\Program Files\Apple\Mobile Device Support\bin"
				End If
			End If
			Environment.SetEnvironmentVariable("Path", String.Join(";", New String() {Environment.GetEnvironmentVariable("Path"), directoryName, ApplicationSupportDirectory.FullName}))
		End Sub

		<DllImport("CoreFoundation.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function __CFStringMakeConstantString(s As Byte()) As Pointer(Of System.Void)
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCConnectionClose(conn As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCConnectionGetFSBlockSize(conn As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCConnectionInvalidate(conn As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCConnectionIsValid(conn As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCConnectionOpen(handle As Pointer(Of System.Void), io_timeout As UInteger, ByRef conn As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCDeviceInfoOpen(conn As Pointer(Of System.Void), ByRef dict As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCDirectoryClose(conn As Pointer(Of System.Void), dir As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCDirectoryCreate(conn As Pointer(Of System.Void), path As String) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCDirectoryOpen(conn As Pointer(Of System.Void), path As Byte(), ByRef dir As Pointer(Of System.Void)) As Integer
		End Function
		Public Shared Function AFCDirectoryOpen(conn As Pointer(Of System.Void), path As String, ByRef dir As Pointer(Of System.Void)) As Integer
			Return AFCDirectoryOpen(conn, Encoding.UTF8.GetBytes(path), dir)
		End Function

		Public Shared Function AFCDirectoryRead(conn As Pointer(Of System.Void), dir As Pointer(Of System.Void), ByRef buffer As String) As Integer
			Dim dirent As Pointer(Of System.Void) = Nothing
			Dim num As Integer = AFCDirectoryRead(conn, dir, dirent)
			If (num = 0) AndAlso (dirent IsNot Nothing) Then
				buffer = Marshal.PtrToStringAnsi(New IntPtr(dirent))
				Return num
			End If
			buffer = Nothing
			Return num
		End Function

		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCDirectoryRead(conn As Pointer(Of System.Void), dir As Pointer(Of System.Void), ByRef dirent As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCFileInfoOpen(conn As Pointer(Of System.Void), path As String, ByRef dict As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCFileRefClose(conn As Pointer(Of System.Void), handle As Long) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCFileRefOpen(conn As Pointer(Of System.Void), path As String, mode As Integer, unknown As Integer, ByRef handle As Long) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCFileRefRead(conn As Pointer(Of System.Void), handle As Long, buffer As Byte(), ByRef len As UInteger) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCFileRefSeek(conn As Pointer(Of System.Void), handle As Long, pos As UInteger, origin As UInteger) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCFileRefSetFileSize(conn As Pointer(Of System.Void), handle As Long, size As UInteger) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCFileRefTell(conn As Pointer(Of System.Void), handle As Long, ByRef position As UInteger) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCFileRefWrite(conn As Pointer(Of System.Void), handle As Long, buffer As Byte(), len As UInteger) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCFlushData(conn As Pointer(Of System.Void), handle As Long) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCKeyValueClose(dict As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCKeyValueRead(dict As Pointer(Of System.Void), ByRef key As Pointer(Of System.Void), ByRef val As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCRemovePath(conn As Pointer(Of System.Void), path As String) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AFCRenamePath(conn As Pointer(Of System.Void), old_path As String, new_path As String) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceActivate(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceConnect(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceCopyDeviceIdentifier(device As Pointer(Of System.Void)) As IntPtr
		End Function
		Public Shared Function AMDeviceCopyValue(device As Pointer(Of System.Void), name As String) As String
			Dim ptr As IntPtr = AMDeviceCopyValue_IntPtr(device, 0, CFStringMakeConstantString(name))
			If ptr <> IntPtr.Zero Then
				Dim len As Byte = Marshal.ReadByte(ptr, 8)
				If len > 0 Then
					Return Marshal.PtrToStringAnsi(New IntPtr(ptr.ToInt64() + 9L), len)
				End If
			End If
			Return String.Empty
		End Function

		<DllImport("iTunesMobileDevice.dll", EntryPoint := "AMDeviceCopyValue", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceCopyValue_IntPtr(device As Pointer(Of System.Void), unknown As UInteger, cfstring As Pointer(Of System.Void)) As IntPtr
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceDeactivate(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceDisconnect(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceEnterRecovery(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceGetConnectionID(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceIsPaired(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceNotificationSubscribe(callback As DeviceNotificationCallback, unused1 As UInteger, unused2 As UInteger, unused3 As UInteger, ByRef am_device_notification_ptr As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceRelease(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceStartService(device As Pointer(Of System.Void), service_name As Pointer(Of System.Void), ByRef handle As Pointer(Of System.Void), unknown As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceStartSession(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceStopSession(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMDeviceValidatePairing(device As Pointer(Of System.Void)) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMRecoveryModeDeviceCopyIMEI(device As AMRecoveryDevice) As String
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMRecoveryModeDeviceCopySerialNumber(device As AMRecoveryDevice) As String
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMRecoveryModeDeviceReboot(device As AMRecoveryDevice) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMRecoveryModeDeviceSetAutoBoot(device As AMRecoveryDevice, paramByte As Byte) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMRestoreModeDeviceCopyIMEI(device As AMRecoveryDevice) As String
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMRestoreModeDeviceCreate(unknown0 As UInteger, connection_id As Integer, unknown1 As UInteger) As Integer
		End Function
		<DllImport("iTunesMobileDevice.dll", CallingConvention := CallingConvention.Cdecl)> _
		Public Shared Function AMRestoreRegisterForDeviceNotifications(dfu_connect As DeviceRestoreNotificationCallback, recovery_connect As DeviceRestoreNotificationCallback, dfu_disconnect As DeviceRestoreNotificationCallback, recovery_disconnect As DeviceRestoreNotificationCallback, unknown0 As UInteger, user_info As Pointer(Of System.Void)) As Integer
		End Function
		Public Shared Function CFStringMakeConstantString(s As String) As Pointer(Of System.Void)
			Return __CFStringMakeConstantString(StringToCString(s))
		End Function

		Public Shared Function CFStringToString(value As Byte()) As String
			Return Encoding.ASCII.GetString(value, 9, value(9))
		End Function

		Public Shared Function StringToCFString(value As String) As Byte()
			Dim bytes As Byte() = New Byte(value.Length + 9) {}
			bytes(4) = 140
			bytes(5) = 7
			bytes(6) = 1
			bytes(8) = CByte(value.Length)
			Encoding.ASCII.GetBytes(value, 0, value.Length, bytes, 9)
			Return bytes
		End Function

		Public Shared Function StringToCString(value As String) As Byte()
			Dim bytes As Byte() = New Byte(value.Length) {}
			Encoding.ASCII.GetBytes(value, 0, value.Length, bytes, 0)
			Return bytes
		End Function
	End Class
End Namespace