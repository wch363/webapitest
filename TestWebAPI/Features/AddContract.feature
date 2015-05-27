Feature: AddVendor
	Add a new vendor and check all fields saved correctly.

@web
Scenario: Add Vendor
	Given Vendor information as following
	| Label      | Value   |
	| VendorID   | test001 |
	| VendorName | test001 |
	| IsActive   | 1       |
	When I press save contract button
	Then A new contract should be added and all fields save correctly