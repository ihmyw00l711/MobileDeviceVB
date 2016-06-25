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
Namespace MobileDeviceVB

	Public Class iPhoneFile
		Inherits Stream
		Private handle As Long
		Private mode As OpenMode
		Private phone As iPhone

		Private Sub New(phone As iPhone, handle As Long, mode As OpenMode)
			Me.phone = phone
			Me.mode = mode
			Me.handle = handle
		End Sub

		Protected Overrides Sub Dispose(disposing As Boolean)
			If disposing AndAlso (Me.handle <> 0L) Then
				MobileDevice.AFCFileRefClose(Me.phone.AFCHandle, Me.handle)
				Me.handle = 0L
			End If
			MyBase.Dispose(disposing)
		End Sub

		Public Overrides Sub Flush()
			MobileDevice.AFCFlushData(Me.phone.AFCHandle, Me.handle)
		End Sub

		Public Shared Function Open(phone As iPhone, path As String, openmode__1 As FileAccess) As iPhoneFile
			Dim num2 As Long
			Dim none As OpenMode = OpenMode.None
			Select Case openmode__1
				Case FileAccess.Read
					none = OpenMode.Read
					Exit Select

				Case FileAccess.Write
					none = OpenMode.Write
					Exit Select

				Case FileAccess.ReadWrite
					Throw New NotImplementedException("Read+Write not (yet) implemented")
			End Select
			Dim str As String = phone.FullPath(phone.CurrentDirectory, path)
			Dim num As Integer = MobileDevice.AFCFileRefOpen(phone.AFCHandle, str, CInt(none), 0, num2)
			If num <> 0 Then
				phone.ReConnect()
				Throw New IOException("AFCFileRefOpen failed with error " + num.ToString())
			End If
			Return New iPhoneFile(phone, num2, none)
		End Function

		Public Shared Function OpenRead(phone As iPhone, path As String) As iPhoneFile
			Return Open(phone, path, FileAccess.Read)
		End Function

		Public Shared Function OpenWrite(phone As iPhone, path As String) As iPhoneFile
			Return Open(phone, path, FileAccess.Write)
		End Function

		Public Overrides Function Read(buffer__1 As Byte(), offset As Integer, count As Integer) As Integer
			Dim buffer2 As Byte()
			If Not Me.CanRead Then
				Throw New NotImplementedException("Stream open for writing only")
			End If
			If offset = 0 Then
				buffer2 = buffer__1
			Else
				buffer2 = New Byte(count - 1) {}
			End If
			Dim len As UInteger = CUInt(count)
			Dim num2 As Integer = MobileDevice.AFCFileRefRead(Me.phone.AFCHandle, Me.handle, buffer2, len)
			If num2 <> 0 Then
				Throw New IOException("AFCFileRefRead error = " + num2.ToString())
			End If
			If buffer2 <> buffer__1 Then
				Buffer.BlockCopy(buffer2, 0, buffer__1, offset, CInt(len))
			End If
			Return CInt(len)
		End Function

		Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
			Dim num As Integer = MobileDevice.AFCFileRefSeek(Me.phone.AFCHandle, Me.handle, CUInt(offset), 0)
			Console.WriteLine("ret = {0}", num)
			Return offset
		End Function

		Public Overrides Sub SetLength(value As Long)
			Dim num As Integer = MobileDevice.AFCFileRefSetFileSize(Me.phone.AFCHandle, Me.handle, CUInt(value))
		End Sub

		Public Overrides Sub Write(buffer__1 As Byte(), offset As Integer, count As Integer)
			Dim buffer2 As Byte()
			If Not Me.CanWrite Then
				Throw New NotImplementedException("Stream open for reading only")
			End If
			If offset = 0 Then
				buffer2 = buffer__1
			Else
				buffer2 = New Byte(count - 1) {}
				Buffer.BlockCopy(buffer__1, offset, buffer2, 0, count)
			End If
			Dim num As Integer = MobileDevice.AFCFileRefWrite(Me.phone.AFCHandle, Me.handle, buffer2, CUInt(count))
		End Sub

		Public Overrides ReadOnly Property CanRead() As Boolean
			Get
				Return (Me.mode = OpenMode.Read)
			End Get
		End Property

		Public Overrides ReadOnly Property CanSeek() As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property CanTimeout() As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property CanWrite() As Boolean
			Get
				Return (Me.mode = OpenMode.Write)
			End Get
		End Property

		Public Overrides ReadOnly Property Length() As Long
			Get
				Throw New Exception("The method or operation is not implemented.")
			End Get
		End Property

		Public Overrides Property Position() As Long
			Get
				Dim position__1 As UInteger = 0
				MobileDevice.AFCFileRefTell(Me.phone.AFCHandle, Me.handle, position__1)
				Return CLng(position__1)
			End Get
			Set
				Me.Seek(value, SeekOrigin.Begin)
			End Set
		End Property

		Private Enum OpenMode
			None = 0
			Read = 2
			Write = 3
		End Enum
	End Class
End Namespace