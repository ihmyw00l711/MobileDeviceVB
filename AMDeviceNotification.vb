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

Imports System.Runtime.InteropServices
Namespace MobileDeviceVB

	<StructLayout(LayoutKind.Sequential, Pack := 1)> _
	Friend Structure AMDeviceNotification
		Private unknown0 As UInteger
		Private unknown1 As UInteger
		Private unknown2 As UInteger
		Private callback As DeviceNotificationCallback
		Private unknown3 As UInteger
	End Structure
End Namespace
