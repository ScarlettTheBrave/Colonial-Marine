# used for Content.Shared\_TacticalOps\Construction\Conditions\TacticalRequiresSkillCondition.cs
construction-category-to-robotics = Робототехника 


# Шаги графа сборки
tactical-construction-step-board-driller = печатную плату «WALL-E»
tactical-construction-step-board-transporter = печатную плату «MULEBot»
tactical-construction-step-board-turret = боевой модуль «CROWS»
tactical-construction-step-m2c-ammo = короб патронов M2C (10x28 мм)
tactical-construction-step-power-cell = элемент питания (батарею)

# Рецепты в меню строительства
tactical-construction-bot-driller-name = Бот-Бурильщик
tactical-construction-bot-driller-desc = Создание бота-бурильщика. (20 Сталь -> Плата «WALL-E» -> Отвертка -> Мультитул -> Батарея -> 10 Пласталь -> Сварка)

tactical-construction-bot-transporter-name = Бот-Перевозчик
tactical-construction-bot-transporter-desc = Создание MULE-бота. (20 Сталь -> Плата «MULEBot» -> Отвертка -> Мультитул -> Батарея -> 10 Пласталь -> Сварка)

tactical-construction-bot-turret-name = Боевой Бот
tactical-construction-bot-turret-desc = Создание боевого CROWS-бота. (20 Сталь -> Плата «CROWS» -> Отвертка -> Мультитул -> Батарея -> 10 Пласталь -> Короб M2C -> Сварка)

# Прототипы плат (автоматическая привязка по ID)
ent-TOCircuitboardDriller = программируемый контроллер-плата «WALL-E»
    .desc = Печатная плата, содержащая логические контуры для автономного инженерного бота-бурильщика.

ent-TOCircuitboardTransporter = программируемая плата «MULEBot»
    .desc = Печатная плата, содержащая логические контуры для автоматизированного транспортного робота.

ent-TOCircuitboardTurret = дистанционно управляемый боевой модуль «CROWS»
    .desc = Боевой модуль для дистанционно управляемого комплекса наземного базирования «CROWS».

# Прототипы каркасов и ботов (автоматическая привязка по ID)
ent-TOBotFrameBase = каркас бота
    .desc = Неоконченный металлический каркас инженерного бота. Видны слоты под проводку, батарею и плату.

ent-TOBotFrameDriller = { ent-TOBotFrameBase }
    .desc = { ent-TOBotFrameBase.desc }

ent-TOBotFrameTransporter = { ent-TOBotFrameBase }
    .desc = { ent-TOBotFrameBase.desc }

ent-TOBotFrameTurret = { ent-TOBotFrameBase }
    .desc = { ent-TOBotFrameBase.desc }

ent-TOBotDriller = бот-бурильщик
    .desc = Автономный инженерный дрон, специализирующийся на бурении породы и расчистке завалов. Был приспособлен под оперативные нужды КМП США и используется в миссиях, где необходима незамедлительная расчистка местности.

ent-TOBotTransporter = бот-перевозчик
    .desc = Грузовая платформа на гусеничном ходу, чья прочность и надежность оставляют желать лучшего. Предназначена для транспортировки ящиков на линию боевого соприкосновения или эвакуации раненых оттуда.

ent-TOBotTurret = боевой бот
    .desc = Мобильный боевой модуль, оснащенный укороченной версией тяжелого пулемета M2C. Базируется на ходовой части транспортного бота.

# Сообщения системы батареи бота
tactical-bot-battery-examine-idle = [color=red]Питание отключено. Бот находится в режиме простоя (Idle).[/color]
tactical-bot-battery-examine-charge = Заряд батареи: [color=yellow]{$percent}%[/color].
tactical-bot-battery-examine-unlocked = [color=cyan]Технический отсек разблокирован (можно поддеть ломом).[/color]
tactical-bot-battery-no-skill = Доступ запрещён! Требуется допуск Продвинутого Инженера.
tactical-bot-battery-unlocked = Вы разблокировали технический отсек бота жетоном.
tactical-bot-battery-locked = Вы заблокировали технический отсек бота жетоном.
tactical-bot-battery-need-id = Технический отсек заблокирован! Сначала проведите жетоном Продвинутого Инженера.
tactical-bot-battery-ejected = Вы поддели крышку ломом и извлекли батарею из бота.
tactical-bot-battery-empty = Внутри технического отсека нет батареи.
tactical-bot-battery-inserted = Вы установили новую батарею в технический отсек бота.

# Сообщения связанные с ноутбуком и связью с ботом
tactical-bot-laptop-linked = Мобильный комплекс «{$bot}» успешно подключён к сети ноутбука.
tactical-bot-laptop-already-linked = Этот бот уже привязан к ноутбуку!

tactical-bot-hud-idle-tag = IDLE
tactical-bot-examine-damage-minor = [color=yellow]На корпусе видны вмятины и следы обгорания.[/color]
tactical-bot-examine-damage-severe = [color=red]Шасси критически повреждено, из-под обшивки сыплются искры![/color]

ent-TOCircuitboardDestroyed = сгоревшая печатная плата робота
    .desc = Обугленная и расколотая печатная плата мобильного робототехнического комплекса. Восстановлению не подлежит.

# Доклад о действиях ботах
tactical-bot-driller-verb-follow = Режим: Следование
tactical-bot-driller-verb-drill = Режим: Бурение 
tactical-bot-driller-mode-follow = Бот-бурильщик переведён в режим сопровождения инженера.
tactical-bot-driller-mode-drill = Бот-бурильщик зафиксирован в режиме проходческого бурения.

# Уведомления касающиеся работоспособности или угрозы уничтожения
tactical-bot-announce-linked = Комплекс «{$bot}» синхронизирован с тактическим терминалом инженера. Все системы в норме.
tactical-bot-announce-no-power = ВНИМАНИЕ: Энергоячейка комплекса «{$bot}» разряжена или извлечена. Переход в режим ожидания.
tactical-bot-announce-critical-damage = ТРЕВОГА: Критическое повреждение бронекорпуса «{$bot}»! Требуется срочный ремонт сваркой!
tactical-bot-announce-drill-complete = ДОКЛАД: Проходческое бурение завершено. Преграда разрушена.
tactical-bot-turret-verb-follow = Режим: Сопровождать меня
tactical-bot-turret-verb-hold = Режим: Удерживать позицию
tactical-bot-turret-mode-follow = Бот-турель переведён в режим сопровождения инженера.
tactical-bot-turret-mode-hold = Бот-турель зафиксирован на текущей огневой позиции.
tactical-bot-turret-reloaded = Боекомплект бота-турели полностью пополнен из короба M2C!
tactical-bot-turret-ammo-full = Боекомплект бота-турели уже полон.

#Анонсы ботов
tactical-announcement-preset-name-engineering-bot = Телеметрия робототехники КМП
tactical-announcement-title-engineering-bot = >> ИНЖЕНЕРНЫЙ <<