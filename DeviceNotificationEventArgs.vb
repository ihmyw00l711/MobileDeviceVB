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

	Public Class DeviceNotificationEventArgs
		Inherits EventArgs
		Private m_device As AMRecoveryDevice

		Friend Sub New(device As AMRecoveryDevice)
			Me.m_device = device
		End Sub

		Friend ReadOnly Property Device() As AMRecoveryDevice
			Get
				Return Me.m_device
			End Get
		End Property
	End Class
End Namespace