# Stories-Marsoc-Locale-Start

# Названия ролей и префиксы
to-job-name-marsoc-officer = Офицер сил специальных операций Корпуса морской пехоты США 
to-job-name-marsoc-tl = Командир группы специальных операций
to-job-name-marsoc-operative = Оперативник группы специальных операций
to-job-name-marsoc-medic = Медик группы специальных операций
to-job-name-marsoc-specialist = Специалист группы специальнных операций

to-job-prefix-marsoc-officer = ОФЦ МРСК
to-job-prefix-marsoc-tl = КГ МРСК
to-job-prefix-marsoc-operative = ОПР МРСК
to-job-prefix-marsoc-medic = МЕД МРСК
to-job-prefix-marsoc-specialist = СПЦ МРСК

to-ghost-role-marsoc-officer-desc = Возглавьте секретную операцию спецподразделения КМП США MARSOC. Изучите директивы командования и разверните свой отряд.
to-ghost-role-marsoc-tl-desc = Руководите штурмовой группой MARSOC непосредственно на поле боя. Выполняйте приказы Офицера операции.
to-ghost-role-marsoc-operative-desc = Вы Элитный боец сил специальных операций MARSOC. Ваша лояльность не поддается сомнению, как и вера в идеалы Америки. Действуйте скрытно и устраняйте приоритетные цели.  
to-ghost-role-marsoc-medic-desc = Вы Боевой медик-хирург отряда MARSOC. Ваша лояльность не поддается сомнению, как и вера в идеалы Америки. Обеспечивайте выживание группы и эвакуацию особо важных объектов.
to-ghost-role-marsoc-specialist-desc = Вы Снайпер-разведчик отряда MARSOC, вооружённый бесшумной крупнокалиберной винтовкой L112. Ваша лояльность не поддается сомнению, как и вера в идеалы Америки.

to-marsoc-rank-officer = Капитан
to-marsoc-rank-lt = Мл. лейтенант
to-marsoc-rank-ssgt = Штаб-сержант
to-marsoc-synth-tag = СИНТ-ЮНИТ

# Приветствие в чате при вселении в тело
to-marsoc-default-officer-callsign = ОВЕРЛОРД-1
to-marsoc-role-welcome-chat = [bold][color=#38BDF8]ДАННЫЕ ОПЕРАТИВНИКА ({$callsign}):[/color][/bold] {$duties}

# Позывные
to-marsoc-callsign-officer-1 = ОВЕРЛОРД-АКТУАЛ
to-marsoc-callsign-officer-2 = ОЛИМП-01
to-marsoc-callsign-officer-3 = ЭГИДА-КОМАНД
to-marsoc-callsign-officer-4 = СПЕКТР-АКТУАЛ

to-marsoc-callsign-tl-1 = СКВАДРОН-1
to-marsoc-callsign-tl-2 = ДАГГЕР-ЛИД
to-marsoc-callsign-tl-3 = ТАЛОН-01
to-marsoc-callsign-tl-4 = ЦЕНТУРИОН-1

to-marsoc-callsign-squad-1 = ДЕЛЬТА-ШЕСТЬ
to-marsoc-callsign-squad-2 = АРК-04
to-marsoc-callsign-squad-3 = СКВАДРОН-6
to-marsoc-callsign-squad-4 = ХАРОН
to-marsoc-callsign-squad-5 = ЦЕРБЕР
to-marsoc-callsign-squad-6 = ВАЛЬКИРИЯ
to-marsoc-callsign-squad-7 = НЕМЕЗИДА
to-marsoc-callsign-squad-8 = ЖНЕЦ-07
to-marsoc-callsign-squad-9 = ФАНТОМ-2
to-marsoc-callsign-squad-10 = КРАКЕН-09

# Снаряжение
ent-TOWeaponRifleXM40Marsoc = штурмовая винтовка XM40
    .desc = Экспериментальный предшественник линейки M54C с интегрированным глушителем, состоящий на вооружении элитных подразделений MARSOC. Укомплектована бронебойным магазином (AP).

ent-TOArmorMARSOCXM40 = тактическая броня M3-SOF (AP)
    .desc = Модифицированный бронекомплект сил специальных операций КМП США. В подсумках размещены 4 бронебойных магазина XM40 AP.

ent-TOArmorMARSOCSniper = тактическая броня M3-SOF (Снайпер)
    .desc = Облегчённый бронекомплект специалиста MARSOC с боекомплектом к винтовке L112.

ent-TOArmorHelmetMARSOC = тактический шлем M3-SOF
    .desc = Бронированный шлем оператора MARSOC со встроенным прибором ночного видения.

ent-TOBeltMedicalMarsocFilled = тактический медицинский пояс M276 Сил Специальных Операций
    .desc = Модифицированный пояс боевого медика ССО чёрного цвета, надёжный, как швейцарские часы. Стоп, а Швейцария вообще существует?...

# Жетоны
ent-TODogtagMarsocOfficer = жетоны Офицера MARSOC
    .desc = Засекреченные идентификационные жетоны командования специальных операций.
ent-TODogtagMarsocOfficerHidden = { ent-TODogtagMarsocOfficer }
    .desc = { ent-TODogtagMarsocOfficer.desc }

ent-TODogtagMarsocTeamLeader = жетоны Командира группы MARSOC
    .desc = Засекреченные идентификационные жетоны специальных операций.
ent-TODogtagMarsocTeamLeaderHidden = { ent-TODogtagMarsocTeamLeader }
    .desc = { ent-TODogtagMarsocTeamLeader.desc }

ent-TODogtagMarsocOperative = жетоны Оперативника MARSOC
    .desc = Засекреченные идентификационные жетоны специальных операций.
ent-TODogtagMarsocOperativeHidden = { ent-TODogtagMarsocOperative }
    .desc = { ent-TODogtagMarsocOperative.desc }

ent-TODogtagMarsocMedic = жетоны Медика MARSOC
    .desc = Засекреченные идентификационные жетоны специальных операций.
ent-TODogtagMarsocMedicHidden = { ent-TODogtagMarsocMedic }
    .desc = { ent-TODogtagMarsocMedic.desc }

ent-TODogtagMarsocSpecialist = жетоны Специалиста MARSOC
    .desc = Засекреченные идентификационные жетоны специальных операций.
ent-TODogtagMarsocSpecialistHidden = { ent-TODogtagMarsocSpecialist }
    .desc = { ent-TODogtagMarsocSpecialist.desc }

# Спавнеры
ent-TOSpawnerMarsocOfficer = роль призрака офицер MARSOC
    .desc = Спавнер командира операции MARSOC.
ent-TOSpawnerMarsocTeamLeader = роль призрака командир группы MARSOC
    .desc = Спавнер младшего лейтенанта MARSOC.
ent-TOSpawnerMarsocOperative = роль призрака оперативник MARSOC
    .desc = Спавнер штурмовика MARSOC.
ent-TOSpawnerMarsocMedic = роль призрака медик MARSOC
    .desc = Спавнер боевого медика MARSOC.
ent-TOSpawnerMarsocSpecialist = роль призрака специалист MARSOC
    .desc = Спавнер снайпера MARSOC с винтовкой L112.

# Действия (Actions)
ent-TOActionMarsocDeploySquad = Развернуть отряд MARSOC
    .desc = Активировать протокол пробуждения боевой группы MARSOC на шаттле.

ent-TOActionMarsocToggleParadrop = Режим десантирования (Шлюзы)
    .desc = Разблокировать внешние шлюзы шаттла и активировать парашютный сброс на поверхность колонии при шаге в открытый космос.

ent-TOActionMarsocConfirmExtraction = Подтвердить эвакуацию (15 сек)
    .desc = Отдать приказ на подъём всех бойцов и целей, находящихся внутри периметра маяка эвакуации.

# Маяк Эвакуации
ent-TOMarsocExtractionBeacon = маяк тактической эвакуации «Скайрейнджер»
    .desc = Устанавливает периметр из сигнальных флаеров для экстренной эвакуации группы MARSOC обратно на орбитальный корвет.

ent-TOMarsocExtractionFlare = сигнальный флаер эвакуации
    .desc = Обозначает границы зоны подбора отряда MARSOC.

to-marsoc-paradrop-enabled = Внешние шлюзы разблокированы! Прыжок из шлюза десантирует бойцов на поверхность планеты.
to-marsoc-paradrop-disabled = Режим десантирования отключён. Выход в открытый космос смертелен!
to-marsoc-beacon-no-permission = Установить маяк эвакуации может только Офицер, Командир группы, Специалист или Синтетик!
to-marsoc-beacon-deployed = Периметр эвакуации развёрнут! Ожидание подтверждения от Командира...
to-marsoc-beacon-confirmed = Эвакуация подтверждена! Всем занять места внутри периметра флаеров - подъём через 15 секунд!
to-marsoc-beacon-extracted = Группа успешно эвакуирована на борт шаттла MARSOC!
to-marsoc-beacon-none-active = Нет развёрнутого маяка эвакуации!
to-marsoc-beacon-on-shuttle-error = Нельзя развернуть маяк эвакуации на борту самого шаттла!
to-marsoc-beacon-aborted-no-operative = Эвакуация отменена: в периметре маяка нет ни одного живого оперативника MARSOC!

# MARSOC BRIEFING CONSOLE
ent-TOComputerMarsocBriefing = терминал специальных директив MARSOC
    .desc = Зашифрованная консоль связи с Высшим Командованием КМП США по Директиве 7-12.

to-marsoc-briefing-window-title = ТЕРМИНАЛ ДИРЕКТИВЫ 7-12 // КАНАЛ СПЕЦОПЕРАЦИЙ MARSOC
to-marsoc-briefing-speaker = КУРАТОР РАЗВЕДКИ КМП США
to-marsoc-briefing-objectives-header = АКТИВНЫЕ ДИРЕКТИВЫ МИССИИ:
to-marsoc-briefing-directive-counter = Профиль директивы: {$current} / {$total}
to-marsoc-briefing-access-denied = Отказано в доступе: требуется уровень допуска Офицера MARSOC.

to-marsoc-briefing-radio-status = [ КАНАЛ СПУТНИКОВОЙ СВЯЗИ: ЗАШИФРОВАНО // ЧАСТОТА 142.85 МГц ]
to-marsoc-briefing-sector-display = Сектор сброса: {$sector}
to-marsoc-briefing-segment-counter = Фрагмент передачи: [ {$current} / {$total} ]

to-marsoc-intel-detected = ПЕРЕХВАТ: активность [{$faction}] (целей: {$count})
to-marsoc-intel-standard = СКАНИРОВАНИЕ ОРБИТЫ: штатный режим

to-marsoc-sector-prefix = Сектор высадки №{$index}
to-marsoc-sector-north = Северный периметр (безопасная зона)
to-marsoc-sector-south = Южный периметр (безопасная зона)
to-marsoc-sector-west = Западный фланг (безопасная зона)
to-marsoc-sector-east = Восточный фланг (безопасная зона)

# Случайные спикеры Командования
to-marsoc-speaker-handler = КУРАТОР ОПЕРАТИВНОГО УПРАВЛЕНИЯ КМП
to-marsoc-speaker-general = КОМАНДУЮЩИЙ СПЕЦОПЕРАЦИЯМИ КМП США
to-marsoc-speaker-director = ДИРЕКТОР СОВЕТА ПО ДИРЕКТИВЕ 7-12
to-marsoc-speaker-random-1 = Генерал-майор София Миллер // Управление специальных операций КМП США
to-marsoc-speaker-random-2 = Полковник София Миллер // Куратор разведки Сектора Неромы
to-marsoc-speaker-random-3 = Бригадный генерал София Миллер // Комитет по Директиве 7-12
to-marsoc-speaker-random-4 = Полковник София Миллер // Штаб 1-го батальона рейдеров КМП

# Кнопки терминала
to-marsoc-briefing-btn-skip = ПРОПУСТИТЬ ТЕКСТ
to-marsoc-briefing-btn-next = СМЕНИТЬ ДИРЕКТИВУ
to-marsoc-briefing-btn-deploy = СПАВН ОТРЯДА
to-marsoc-briefing-btn-deployed = ОТРЯД УЖЕ РАЗВЁРНУТ
to-marsoc-briefing-btn-paradrop-off = ОТКРЫТЬ ШЛЮЗЫ (НАЧАТЬ СБРОС)
to-marsoc-briefing-btn-paradrop-on = БЛОКИРОВАТЬ ШЛЮЗЫ
to-marsoc-briefing-btn-sector = СМЕНИТЬ СЕКТОР СБРОСА
to-marsoc-briefing-btn-lock-sector = ПОДТВЕРДИТЬ СЕКТОР СБРОСА
to-marsoc-briefing-btn-sector-locked = СЕКТОР СБРОСА ЗАФИКСИРОВАН
to-marsoc-briefing-btn-launch = ВЫЛЕТ К ЦЕЛИ
to-marsoc-briefing-btn-in-transit = В ПОЛЁТЕ К ЦЕЛИ ({$seconds} СЕК)
to-marsoc-briefing-btn-arrived = НА ОРБИТЕ СЕКТОРА
to-marsoc-briefing-btn-flight-on = ХОД ШАТТЛА: ОРБИТАЛЬНЫЙ ПОЛЁТ
to-marsoc-briefing-btn-flight-off = ХОД ШАТТЛА: ТИХИЙ ДРЕЙФ
to-marsoc-briefing-btn-broadcast = ПЕРЕДАТЬ ДИРЕКТИВУ ОТРЯДУ

# Всплывающие уведомления терминала
to-marsoc-flight-not-arrived = Дождитесь выхода корвета на орбиту целевого сектора!
to-marsoc-sector-not-locked = Сначала выберите и подтвердите зону сброса!

# Баннер операции (Титры)
to-marsoc-banner-operation-default = OPERATION BLACKOUT-SPEAR
to-marsoc-banner-assignment = 1st Marine Raider Battalion "MARSOC"

# Директивы и Миссии (с {$officer} для подстановки позывного Офицера)
to-marsoc-mission-trijent-title = ОПЕРАЦИЯ «КАЗУС БЕЛЛИ» // ДАМБА ТРИДЖЕНТ
to-marsoc-mission-trijent-dialogue = Внимание, {$officer}. Над колонией Дамба Триджент был сбит борт 2-го взвода 173-й воздушно-десантной роты СПН. Они шли за образцами биооружия Ве-Я. Официально нас здесь нет, {$officer}. Если десантники СПН попадут в руки обычных морпехов с «Алмайера», дипломаты замнут инцидент. Высадитесь, найдите выживших бойцов СПН и ликвидируйте их. Нам нужна эта война.
to-marsoc-mission-trijent-obj-1 = Ликвидировать выживших десантников 173-й роты ВДВ СПН на Дамбе Триджент.
to-marsoc-mission-trijent-obj-2 = Эвакуировать жетоны СПН через периметр маяка «Скайрейнджер».

to-marsoc-mission-lv624-title = ОПЕРАЦИЯ «ЧЁРНЫЙ КУПОЛ» // LV-624
to-marsoc-mission-lv624-dialogue = Исполнительный специалист Ве-Я уровня WYC5 забаррикадировался в корпокуполе LV-624 после обнаружения яиц в северном храме, {$officer}. Из паранойи его охрана расстреляла гражданских колонистов, приняв их за боевиков КОФ. Командование требует рычаг давления на совет директоров Ве-Я. Пробейтесь в купол, {$officer}, и доставьте WYC5 живым на борт корвета.
to-marsoc-mission-lv624-obj-1 = Захватить исполнительного специалиста WYC5 или Связного живым.
to-marsoc-mission-lv624-obj-2 = Доставить цель в периметр флаеров маяка эвакуации и поднять на шаттл.

to-marsoc-mission-hybrisa-title = ОПЕРАЦИЯ «ЧУЖОЙ ФЛАГ» // ГИБРИСА-ПРОСПЕРА
to-marsoc-mission-hybrisa-dialogue = Прямое включение, {$officer}. Ве-Я выкупила военную базу ИТМ на Гибрисе под офис своего супервайзера, но охрана корпоратов проиграла улью в первый же день. Сейчас остатки 21-й Имперской парашютной дивизии пытаются удержать периметр. Изымите секретные данные исследований из лаборатории Ве-Я, а ответственность за саботаж возложите на королевских морпехов ИТМ, {$officer}.
to-marsoc-mission-hybrisa-obj-1 = Изъять носители данных из исследовательского комплекса Гибрисы.
to-marsoc-mission-hybrisa-obj-2 = Зачистить свидетелей корпоративной охраны Ве-Я в секторе.

to-marsoc-mission-almayer-title = ДИРЕКТИВА «ВНУТРЕННИЙ КОНТУР» // УДКТ «АЛМАЙЕР»
to-marsoc-mission-almayer-dialogue = Канал зашифрован, {$officer}. Политическое крыло Командования недовольно фракционной позицией Командующего Офицера «Алмайера». Ваша группа прикомандирована в качестве теневого надзора. Проследите за ходом операции: если Командующий провалит высадку или попадёт в захват улья — вмешайтесь, {$officer}, и проведите принудительную эвакуацию либо отстранение.
to-marsoc-mission-almayer-obj-1 = Обеспечить выполнение директивы фракции в отношении командования «Алмайера».
to-marsoc-mission-almayer-obj-2 = Вернуться на борт шаттла MARSOC до окончания операции.

# Дополнительные генерируемые задачи
to-marsoc-mission-opt-1 = Обеспечить полную секретность присутствия группы перед рядовым составом КМП.
to-marsoc-mission-opt-2 = Сохранить жизнь минимум 80% личного состава отряда до завершения эвакуации.
to-marsoc-mission-opt-3 = Собрать и доставить на борт корвета данные тактической разведки из сектора высадки.


# Точки сброса под открытым небом на Гибрисе (по координатам карты)
to-marsoc-sector-hybrisa-nw-checkpoint = Улицы Колонии — СЗ КПП (25, 189)
to-marsoc-sector-hybrisa-nw-lot = Улицы Колонии — СЗ Автопарк (56, 230)
to-marsoc-sector-hybrisa-ne-streets = Улицы Колонии — СВ Периметр (188, 236)

# Статусы сканера инсертов
to-marsoc-intel-tse-survivors = ПЕРЕХВАТ: Авиабаза ИТМ // гарнизон парашютистов Трёх Солнц
to-marsoc-intel-tse-airbase = ПЕРЕХВАТ: Авиабаза ИТМ // комплекс оставлен персоналом
to-marsoc-intel-clf-spaceport = ПЕРЕХВАТ: Космопорт // укреплённый узел боевиков КОФ
to-marsoc-intel-trijent = ПЕРЕХВАТ: Дамба Триджент // сигнал десанта СПН
to-marsoc-intel-lv624 = ПЕРЕХВАТ: LV-624 // бункер руководителя WYC6

# Динамическая миссия для инсерта clf_spaceport.yml
to-marsoc-mission-clf-spaceport-title = ОПЕРАЦИЯ «ЖЕЛЕЗНЫЙ ПРИЧАЛ» // КОСМОПОРТ КОФ
to-marsoc-mission-clf-spaceport-dialogue = Прямое включение, {$officer}. Боевики Колониального Освободительного Фронта захватили Стыковочный Док №2 космопорта Гибрисы и развернули модифицированные гаусс-турели. В их штабе удерживается спецкейс D18 и ключи шифрования колонии. Высадитесь в уличном секторе, {$officer}, вскройте оборону космопорта и изымите кейс D18.
to-marsoc-mission-clf-spaceport-obj-1 = Зачистить Стыковочный Док №2 космопорта и подавить гаусс-турели боевиков КОФ.
to-marsoc-mission-clf-spaceport-obj-2 = Изъять спецкейс D18 и ключи шифрования колонии из штаба КОФ для эвакуации на шаттл.

# Динамическая миссия для инсерта tse_airbase.yml 
to-marsoc-mission-tse-airbase-title = ОПЕРАЦИЯ «ТИХИЙ АНГАР» // АВИАБАЗА ИТМ
to-marsoc-mission-tse-airbase-dialogue = На связи Командование, {$officer}. Спутниковая разведка зафиксировала, что гарнизон авиабазы Империи Трёх Солнц на Гибрисе экстренно покинул периметр, оставив в диспетчерской бортовой самописец и книгу Имперских кодов аутентификации. Высадитесь под открытым небом у СЗ сектора, {$officer}, проникните в ангар ИТМ и эвакуируйте секретные носители до подхода основных сил КМП.
to-marsoc-mission-tse-airbase-obj-1 = Проникнуть в диспетчерскую и оружейный блок авиабазы ИТМ (TSE Airbase).
to-marsoc-mission-tse-airbase-obj-2 = Забрать Имперские коды аутентификации и бортовой самописец колонии на борт корвета.

# Вторичные динамические задачи от малых инсертов выживших 
to-marsoc-subinsert-meridian-obj = Обследовать укреплённый сектор «Меридиан» и изъять данные с компьютера идентификации.
to-marsoc-subinsert-apartments-obj = Обыскать баррикады в жилых апартаментах и изъять кассетные записи Связного Ве-Я.
to-marsoc-subinsert-bar-obj = Зачистить укреплённый бар колонистов и ликвидировать вооружённых свидетелей.

to-marsoc-beacon-under-roof-error = Маяк эвакуации можно развернуть только под открытым небом!

ent-TOMarsocDropZoneBase = маркер высадки MARSOC
    .desc = Точка парашютного десантирования отряда MARSOC.
    .suffix = MARSOC, Высадка

ent-TOMarsocDropZoneMarkerAlpha = маркер высадки MARSOC (Сектор 1)
    .desc = { ent-TOMarsocDropZoneBase.desc }
    .suffix = MARSOC, Высадка, Сектор 1

ent-TOMarsocDropZoneMarkerBravo = маркер высадки MARSOC (Сектор 2)
    .desc = { ent-TOMarsocDropZoneBase.desc }
    .suffix = MARSOC, Высадка, Сектор 2

ent-TOMarsocDropZoneMarkerCharlie = маркер высадки MARSOC (Сектор 3)
    .desc = { ent-TOMarsocDropZoneBase.desc }
    .suffix = MARSOC, Высадка, Сектор 3

ent-TOMarsocDropZoneMarkerDelta = маркер высадки MARSOC (Сектор 4)
    .desc = { ent-TOMarsocDropZoneBase.desc }
    .suffix = MARSOC, Высадка, Сектор 4

ent-TOMarsocBunkerControllerMarker = маркер бункера Ве-Я и спавна Директора
    .desc = Спавнит Супервайзера Ве-Я и управляет таймером ставней бункера.
    .suffix = MARSOC, Бункер Ве-Я

ent-TOMarsocLoreReport1 = научный отчет лаборатории №1
    .desc = Документ исследовательского комплекса Ве-Я.
    .suffix = MARSOC, Лор

ent-TOMarsocLoreReport2 = научный отчет лаборатории №2
    .desc = Документ исследовательского комплекса Ве-Я.
    .suffix = MARSOC, Лор

ent-TOMarsocLoreReport3 = научный отчет лаборатории №3
    .desc = Документ исследовательского комплекса Ве-Я.
    .suffix = MARSOC, Лор

ent-TOMarsocLoreReport4 = научный отчет лаборатории №4
    .desc = Документ исследовательского комплекса Ве-Я.
    .suffix = MARSOC, Лор

ent-TOMarsocWeYaBlackLedger = черная бухгалтерия Ве-Я
    .desc = Финансовая документация руководства колонии.
    .suffix = MARSOC, Квест

ent-TOMarsocWeYaResearchData = данные исследований лаборатории
    .desc = Отчет о работе с образцами в криокапсуле.
    .suffix = MARSOC, Квест

ent-TOMarsocExcavationJournal = журнал раскопок
    .desc = Полевой дневник археологов из шахт Гибрисы.
    .suffix = MARSOC, Квест

ent-TOMarsocWeYaCommandProtocols = командные протоколы Ве-Я
    .desc = Коды вызова группы быстрого реагирования ЧВК через ретранслятор.
    .suffix = MARSOC, Квест

ent-TOMarsocAbominationFlask = колба с биообразцом
    .desc = Хрупкий стеклянный сосуд с живым паразитом внутри.
    .suffix = MARSOC, Квест

to-marsoc-bunker-countdown-started = Запущен протокол разблокировки бункера. Ставни откроются через {$minutes} мин.
to-marsoc-bunker-countdown-active = Протокол разблокировки уже запущен. Осталось {$seconds} сек.
to-marsoc-bunker-shutters-opened = Ставни бункера Ве-Я разблокированы и открыты.

to-marsoc-cryopod-extracted = Вы выгрузили данные исследований из криокапсулы.
to-marsoc-cryopod-already-extracted = Данные с этой криокапсулы уже были скачаны.
to-marsoc-case-dispensed = Кейс хранения открывает секретный отсек с документами.
to-marsoc-protocols-activated = Командные протоколы активированы. Сигнал отправлен на орбиту.

# Тексты бумажек документации
to-marsoc-lore-report-1-text = Запись 1.
to-marsoc-lore-report-2-text = Запись 2.
to-marsoc-lore-report-3-text = Запись 3.
to-marsoc-lore-report-4-text = Запись 4.
to-marsoc-black-ledger-text = Документация черной бухгалтерии.
to-marsoc-research-data-text = Данные исследований из криокапсулы.
to-marsoc-excavation-journal-text = Записи раскопок в пещерах.
to-marsoc-command-protocols-text = Коды вызова ЧВК Ве-Я.

to-marsoc-para-objective-1 = Захватить или ликвидировать Супервайзера Ве-Я в бункере.
to-marsoc-para-objective-2 = Забрать данные исследований с большой криокапсулы в восточной лаборатории.
to-marsoc-para-objective-3 = Вскрыть кейс хранения Ве-Я в бункере и эвакуировать секретные документы.
to-marsoc-para-briefing-chat = Задача вашего подразделения на высадку - {$objective}

# Кому подчиняются роли и что нужно сделать. Вместо затычки nobody в чат
to-marsoc-supervisors-tl = Офицеру группы специальных операций 
to-marsoc-supervisors-squad = Офицеру и Командиру группы специальных операций MARSOC
to-marsoc-supervisors-exec = Совету директоров Вестон-Ямада и особым корпоративным директивам
to-marsoc-supervisors-assistant = Руководителю корпоративного управления и секретным директивам Вестон-Ямада

# Заголовки и задачи для вкладки Персонаж (клавиша C)
to-marsoc-charinfo-header = Директивы группы специальных операций MARSOC
to-marsoc-charinfo-default-obj = Выполнять боевые приказы Офицера операции в секторе высадки.

# Рандомные задачи Супервайзера (выбирается одна при спавне в зависимости от предметов)
to-marsoc-supervisor-obj-ledger = Директива Вестон-Ямада. Основная задача выжить и покинуть колонию. Приоритетная цель найти в бункере и эвакуировать черную бухгалтерию компании.
to-marsoc-supervisor-obj-research = Директива Вестон-Ямада. Основная задача выжить и покинуть колонию. Приоритетная цель снять и эвакуировать данные исследований с большой криокапсулы в восточной лаборатории.
to-marsoc-supervisor-obj-flask = Директива Вестон-Ямада. Основная задача выжить и покинуть колонию. Приоритетная цель забрать из кейса хранения хрупкую колбу с биообразцом и доставить ее целой.
to-marsoc-supervisor-obj-journal = Директива Вестон-Ямада. Основная задача выжить и покинуть колонию. Приоритетная цель раздобыть полевой журнал раскопок и активировать командные протоколы при наличии связи.

# Рандомные задачи Помощника Менеджера (50/50 помощь или устранение)
to-marsoc-assistant-obj-help = Директива Вестон-Ямада. Основная задача выжить. Помогите Руководителю корпоративного управления собрать секретные документы комплекса и обеспечьте его эвакуацию.
to-marsoc-assistant-obj-kill = Секретная директива кадрового отдела Вестон-Ямада. Основная задача выжить. Дождитесь момента, когда Руководитель корпоративного управления соберет документы комплекса, после чего ликвидируйте его и эвакуируйтесь с материалами единолично для получения повышения.

department-TOMarsoc = Силы специальных операций КМП (MARSOC)
department-TOMarsoc-description = Засекреченный контингент 1-го батальона рейдеров КМП США, подчиняющийся напрямую Директиве 7-12.

to-marsoc-exposure-console-status = Уровень обнаружения отряда {$percent}%
to-marsoc-exposure-los-warning = Внимание! Морпехи наблюдают за вами. Немедленно разорвите зрительный контакт!
to-marsoc-exposure-reason-operative-dead = Гибель оперативника MARSOC в секторе
to-marsoc-exposure-reason-marine-killed = Ликвидация морского пехотинца ККМП
to-marsoc-exposure-squad-alert = ТРЕВОГА СЕКРЕТНОСТИ ({$reason}). Уровень обнаружения отряда вырос до {$percent}%!
to-marsoc-exposure-ares-75-warning = Внимание всем подразделениям. Зафиксировано присутствие неизвестной вооруженной группы без транспондеров ККМП в зоне проведения операции.
to-marsoc-exposure-squad-75-warning = КРИТИЧЕСКАЯ УГРОЗА! Уровень обнаружения превысил 75%. АРЕС запеленговал активность группы в секторе!
to-marsoc-exposure-ares-100-compromised = Экстренное оповещение АРЕС. Батареи ПВО Алмайера обнаружили и поразили неопознанный корвет на низкой орбите. Корпорации Вестон-Ямада направлен подробный отчёт о произошедшем, а так же полученые даные.
to-marsoc-exposure-squad-100-compromised = ПРОВАЛ МАСКИРОВКИ (100%). Орбитальный корвет поражен огнем ПВО Алмайера и экстренно покинул орбиту! Маяки эвакуации отключены!