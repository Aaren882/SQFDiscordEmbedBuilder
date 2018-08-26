/*──────────────────────────────────────────────────────┐
│   Author: Connor                                      │
│   Steam:  https://steamcommunity.com/id/_connor       │
│   Github: https://github.com/ConnorAU                 │
│                                                       │
│   Please do not modify or remove this comment block   │
└──────────────────────────────────────────────────────*/

class CfgDiscordEmbedTemplate {
    title="";
    description="";
    url="";
    color="";
    timestamp=0;
    thumbnail="";
    image="";
    class Author {
        name="";
        url="";
        image="";
    };
    class Footer {
        text="";
        image="";
    }; 
    // Maximum 25 fields per embed
    fields[]={
        //{title,content,inline}
    };
};
class CfgDiscordEmbedBuilder {
	#include "embeds\Example.cpp"
	#include "embeds\Example2.cpp"
};
class CfgDiscordEmbedWebhooks {
    // https://discordapp.com/api/webhooks/000000000000000000/XXXXXXX_XXXXXXXXXXXXXXX_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    //https://ptb.discordapp.com/api/webhooks/483082309717721089/UV1JW1DB9_M1YhxFbLdztLUclqiCIUUgmV45vaKoyb_eaagx6zh0ZsxpzvPa-v7v7Y72
    Example="483082309717721089/UV1JW1DB9_M1YhxFbLdztLUclqiCIUUgmV45vaKoyb_eaagx6zh0ZsxpzvPa-v7v7Y72";
};



// You don't need to worry about this stuff
class CfgPatches {
	class DiscordEmbedBuilder {
        name="DiscordEmbedBuilder";
        author="Connor";
        url="https://steamcommunity.com/id/_connor";

		requiredVersion=1.68;
		requiredAddons[]={};
		units[]={};
		weapons[]={};
	};
};
class CfgFunctions {
	class DiscordEmbedBuilder {
		class Scripts {
            file = "\DiscordEmbedBuilder";
			class init {preInit=1;};
			class buildCfg {};
			class buildSqf {};
			class send {};
		};
	};
};