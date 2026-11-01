# used for Content.Shared\_TacticalOps\Construction\Conditions\TacticalRequiresSkillCondition.cs
# Construction Categories
construction-category-to-robotics = Robotics (ST)

# Construction Conditions
tactical-construction-requires-skill = Requires skill: Advanced Engineering

# Robotics Construction Steps
tactical-construction-step-board-driller = "WALL-E" drillbot circuitboard
tactical-construction-step-board-transporter = "MULEBot" transporter circuitboard
tactical-construction-step-board-turret = "CROWS" turret bot circuitboard
tactical-construction-step-m2c-ammo = M2C ammunition box (10x28mm)

# Construction menu recipes
tactical-construction-bot-driller-name = Driller Bot
tactical-construction-bot-driller-desc = Creation of a driller bot. (20 Steel -> "WALL-E" Board -> Screwdriver -> Multitool -> Battery -> 10 Plasteel -> Welder)

tactical-construction-bot-transporter-name = Transporter Bot
tactical-construction-bot-transporter-desc = Creation of a MULE bot. (20 Steel -> "MULEBot" Board -> Screwdriver -> Multitool -> Battery -> 10 Plasteel -> Welder)

tactical-construction-bot-turret-name = Combat Bot
tactical-construction-bot-turret-desc = Creation of a combat CROWS bot. (20 Steel -> "CROWS" Board -> Screwdriver -> Multitool -> Battery -> 10 Plasteel -> M2C Ammo Box -> Welder)

# Board prototypes (automatic ID binding)
ent-TOCircuitboardDriller = "WALL-E" programmable controller board
    .desc = A circuit board containing the logic circuits for an autonomous driller engineering bot.

ent-TOCircuitboardTransporter = "MULEBot" programmable board
    .desc = A circuit board containing the logic circuits for an automated transport robot.

ent-TOCircuitboardTurret = "CROWS" remote-controlled combat module
    .desc = A combat module for the "CROWS" ground-based remote-controlled complex.

# Frame and bot prototypes (automatic ID binding)
ent-TOBotFrameBase = bot frame
    .desc = An unfinished metal frame of an engineering bot. Slots for wiring, a battery, and a circuit board are visible.

ent-TOBotFrameDriller = { ent-TOBotFrameBase }
    .desc = { ent-TOBotFrameBase.desc }

ent-TOBotFrameTransporter = { ent-TOBotFrameBase }
    .desc = { ent-TOBotFrameBase.desc }

ent-TOBotFrameTurret = { ent-TOBotFrameBase }
    .desc = { ent-TOBotFrameBase.desc }

ent-TOBotDriller = driller bot
    .desc = An autonomous engineering drone specializing in rock drilling and clearing debris. It has been adapted for USCM operational needs and is used in missions requiring immediate terrain clearance.

ent-TOBotTransporter = transporter bot
    .desc = A tracked cargo platform whose durability and reliability leave much to be desired. Designed to transport crates to the line of contact or evacuate the wounded from it.

ent-TOBotTurret = combat bot
    .desc = A mobile combat module equipped with a shortened version of the M2C heavy machine gun. Based on the undercarriage of the transporter bot.

# Bot battery system messages
tactical-bot-battery-examine-idle = [color=red]Power is off. The bot is in Idle mode.[/color]
tactical-bot-battery-examine-charge = Battery charge: [color=yellow]{$percent}%[/color].
tactical-bot-battery-examine-unlocked = [color=cyan]The maintenance hatch is unlocked (can be pried open with a crowbar).[/color]
tactical-bot-battery-no-skill = Access denied! Advanced Engineer clearance required.
tactical-bot-battery-unlocked = You unlocked the bot's maintenance hatch with your ID card.
tactical-bot-battery-locked = You locked the bot's maintenance hatch with your ID card.
tactical-bot-battery-need-id = The maintenance hatch is locked! Swipe an Advanced Engineer ID card first.
tactical-bot-battery-ejected = You pried the cover with a crowbar and removed the battery from the bot.
tactical-bot-battery-empty = There is no battery inside the maintenance hatch.
tactical-bot-battery-inserted = You installed a new battery into the bot's maintenance hatch.

# Messages related to laptop and bot connection
tactical-bot-laptop-linked = Mobile complex "{$bot}" successfully connected to the laptop's network.
tactical-bot-laptop-already-linked = This bot is already linked to the laptop!

tactical-bot-hud-idle-tag = IDLE
tactical-bot-examine-damage-minor = [color=yellow]Dents and scorch marks are visible on the hull.[/color]
tactical-bot-examine-damage-severe = [color=red]The chassis is critically damaged, sparks are flying from under the plating![/color]

ent-TOCircuitboardDestroyed = burned out robot circuit board
    .desc = A charred and cracked circuit board of a mobile robotic complex. It cannot be repaired.

# Bot action reports
tactical-bot-driller-verb-follow = Mode: Follow
tactical-bot-driller-verb-drill = Mode: Drill 
tactical-bot-driller-mode-follow = The driller bot has been switched to engineer escort mode.
tactical-bot-driller-mode-drill = The driller bot is locked in tunneling drill mode.

# Notifications regarding operational status or destruction threat
tactical-bot-announce-linked = The "{$bot}" complex is synchronized with the engineer's tactical terminal. All systems nominal.
tactical-bot-announce-no-power = WARNING: The "{$bot}" complex power cell is depleted or removed. Entering standby mode.
tactical-bot-announce-critical-damage = ALERT: Critical hull damage to "{$bot}"! Urgent welding repair required!
tactical-bot-announce-drill-complete = REPORT: Tunneling drill complete. Obstacle destroyed.
tactical-bot-turret-verb-follow = Mode: Escort Me
tactical-bot-turret-verb-hold = Mode: Hold Position
tactical-bot-turret-mode-follow = The turret bot has been switched to engineer escort mode.
tactical-bot-turret-mode-hold = The turret bot is locked in its current firing position.
tactical-bot-turret-reloaded = Turret bot ammunition fully replenished from the M2C ammo box!
tactical-bot-turret-ammo-full = Turret bot ammunition is already full.

# Bot announcements
tactical-announcement-preset-name-engineering-bot = USCM Robotics Telemetry
tactical-announcement-title-engineering-bot = >> ENGINEERING <<