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

/***************************
//- Embeds
Title -                                        "Title"
Description -                                  "DESC"
Color -                                        ""
AuthorName -                                   profileName
AuthorUrl -                                    "https://steamcommunity.com/id/_connor"
AuthorIconUrl -                                "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/1e/1e3c83b65d6f34cc9708eae853e8bc9848865dd1_full.jpg"
ImageUrl ["Http(s)://" OR "attachment://"] -   "https://arma3.com/assets/img/wallpapers/low/3/Arma%203%20Laws%20of%20War_wallpaper_1024x768.jpg"
ThumbnailUrl -                                 "https://arma3.com/assets/img/wallpapers/1/9/arma3_white_plain_800x600.jpg"
FooterText -                                   "This is the footer text"
FooterIconUrl -                                "https://steamcdn-a.opskins.media/steamcommunity/public/images/apps/107410/3212af52faf994c558bd622cb0f360c1ef295a6b.jpg"
***************************/

"DiscordEmbedBuilder" callExtension [
 call DiscordEmbedBuilder_SessionKey,
 [
  "https://discord.com/api/webhooks/1236040735065767936/9fJ6Joxk6zAZ5d8WNTNh3RTpnWVV1PlVMJYJNlua3cpCTnJj6x2YsSMNWTcBZeLE12XO",
  "WW",
  "",
  "",
  false,
	"P:/MapLegend.png",
	[
		[
			"Title",
			"DESC",
			"14177041",
			profileName,
			"https://steamcommunity.com/id/_connor",
			"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/1e/1e3c83b65d6f34cc9708eae853e8bc9848865dd1_full.jpg",
			"https://arma3.com/assets/img/wallpapers/low/3/Arma%203%20Laws%20of%20War_wallpaper_1024x768.jpg",
			"https://arma3.com/assets/img/wallpapers/1/9/arma3_white_plain_800x600.jpg",
			"This is the footer text",
			"https://steamcdn-a.opskins.media/steamcommunity/public/images/apps/107410/3212af52faf994c558bd622cb0f360c1ef295a6b.jpg"
		]
	]
 ]
]