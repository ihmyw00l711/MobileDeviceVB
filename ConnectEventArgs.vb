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

Namespace MobileDeviceVB

	Public Class ConnectEventArgs
		Inherits EventArgs
		Private m_device As Pointer(Of System.Void)
		Private m_message As NotificationMessage

		Friend Sub New(cbi As AMDeviceNotificationCallbackInfo)
			Me.m_message = cbi.msg
			Me.m_device = cbi.dev
		End Sub

		Public ReadOnly Property Device() As Pointer(Of System.Void)
			Get
				Return Me.m_device
			End Get
		End Property

		Public ReadOnly Property Message() As NotificationMessage
			Get
				Return Me.m_message
			End Get
		End Property
	End Class
End Namespace