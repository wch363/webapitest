Feature: GetCurrentUser
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: GetCurrentUser
	When Send a request to api:'Authentication/GetCurrentUser'
	Then Information of current user should be returned
