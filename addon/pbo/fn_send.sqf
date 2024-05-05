/* ----------------------------------------------------------------------------
Project:
	https://github.com/ConnorAU/SQFDiscordEmbedBuilder

Author:
	ConnorAU - https://github.com/ConnorAU

Function:
	DiscordEmbedBuilder_fnc_send

Description:
	Sends a built message to the extension to execute a discord webhook

Return:
	BOOL - true if the message was sent to the extension
---------------------------------------------------------------------------- */

if !(uiNamespace getVariable ["DiscordEmbedBuilder_LoadSuccess",false]) exitwith {false};

params [
	["_webhookurl","",[""]],
	["_message","",[""]],
	["_username","",[""]],
	["_avatar","",[""]],
	["_tts",false,[true]],
	["_embeds",[],[[]]]
];

private _key = uiNamespace getVariable ["DiscordEmbedBuilder_SessionKey",{""}];
"DiscordEmbedBuilder" callExtension [call _key,[_webhookurl,_message,_username,_avatar,_tts,_embeds]];
true


"DiscordEmbedBuilder" callExtension [ 
 call DiscordEmbedBuilder_SessionKey,  
 [ 
  "https://discord.com/api/webhooks/1236040735065767936/9fJ6Joxk6zAZ5d8WNTNh3RTpnWVV1PlVMJYJNlua3cpCTnJj6x2YsSMNWTcBZeLE12XO", 
  "WW", 
  "", 
  "", 
  false,
	"" ,
  format ['"%1": "%2"',"title","TITLE"]
 ] 
];