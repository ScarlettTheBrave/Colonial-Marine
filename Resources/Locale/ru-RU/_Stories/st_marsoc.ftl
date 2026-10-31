# Stories-Marsoc-Locale-Start

# Названия ролей и префиксы
st-job-name-marsoc-officer = Офицер сил специальных операций Корпуса морской пехоты США 
st-job-name-marsoc-tl = Командир группы специальных операций
st-job-name-marsoc-operative = Оперативник группы специальных операций
st-job-name-marsoc-medic = Медик группы специальных операций
st-job-name-marsoc-specialist = Специалист группы специальнных операций

st-job-prefix-marsoc-officer = ОФЦ МРСК
st-job-prefix-marsoc-tl = КГ МРСК
st-job-prefix-marsoc-operative = ОПР МРСК
st-job-prefix-marsoc-medic = МЕД МРСК
st-job-prefix-marsoc-specialist = СПЦ МРСК

st-ghost-role-marsoc-officer-desc = Возглавьте секретную операцию спецподразделения КМП США MARSOC. Изучите директивы командования и разверните свой отряд.
st-ghost-role-marsoc-tl-desc = Руководите штурмовой группой MARSOC непосредственно на поле боя. Выполняйте приказы Офицера операции.
st-ghost-role-marsoc-operative-desc = Вы Элитный боец сил специальных операций MARSOC. Ваша лояльность не поддается сомнению, как и вера в идеалы Америки. Действуйте скрытно и устраняйте приоритетные цели.  
st-ghost-role-marsoc-medic-desc = Вы Боевой медик-хирург отряда MARSOC. Ваша лояльность не поддается сомнению, как и вера в идеалы Америки. Обеспечивайте выживание группы и эвакуацию особо важных объектов.
st-ghost-role-marsoc-specialist-desc = Вы Снайпер-разведчик отряда MARSOC, вооружённый бесшумной крупнокалиберной винтовкой L112. Ваша лояльность не поддается сомнению, как и вера в идеалы Америки.

st-marsoc-rank-officer = Капитан
st-marsoc-rank-lt = Мл. лейтенант
st-marsoc-rank-ssgt = Штаб-сержант
st-marsoc-synth-tag = СИНТ-ЮНИТ

# Приветствие в чате при вселении в тело
st-marsoc-default-officer-callsign = ОВЕРЛОРД-1
st-marsoc-role-welcome-chat = [bold][color=#38BDF8]ДАННЫЕ ОПЕРАТИВНИКА ({$callsign}):[/color][/bold] {$duties}

# Позывные
st-marsoc-callsign-officer-1 = ОВЕРЛОРД-АКТУАЛ
st-marsoc-callsign-officer-2 = ОЛИМП-01
st-marsoc-callsign-officer-3 = ЭГИДА-КОМАНД
st-marsoc-callsign-officer-4 = СПЕКТР-АКТУАЛ

st-marsoc-callsign-tl-1 = СКВАДРОН-1
st-marsoc-callsign-tl-2 = ДАГГЕР-ЛИД
st-marsoc-callsign-tl-3 = ТАЛОН-01
st-marsoc-callsign-tl-4 = ЦЕНТУРИОН-1

st-marsoc-callsign-squad-1 = ДЕЛЬТА-ШЕСТЬ
st-marsoc-callsign-squad-2 = АРК-04
st-marsoc-callsign-squad-3 = СКВАДРОН-6
st-marsoc-callsign-squad-4 = ХАРОН
st-marsoc-callsign-squad-5 = ЦЕРБЕР
st-marsoc-callsign-squad-6 = ВАЛЬКИРИЯ
st-marsoc-callsign-squad-7 = НЕМЕЗИДА
st-marsoc-callsign-squad-8 = ЖНЕЦ-07
st-marsoc-callsign-squad-9 = ФАНТОМ-2
st-marsoc-callsign-squad-10 = КРАКЕН-09

# Снаряжение
ent-STWeaponRifleXM40Marsoc = штурмовая винтовка XM40
    .desc = Экспериментальный предшественник линейки M54C с интегрированным глушителем, состоящий на вооружении элитных подразделений MARSOC. Укомплектована бронебойным магазином (AP).

ent-STArmorMARSOCXM40 = тактическая броня M3-SOF (AP)
    .desc = Модифицированный бронекомплект сил специальных операций КМП США. В подсумках размещены 4 бронебойных магазина XM40 AP.

ent-STArmorMARSOCSniper = тактическая броня M3-SOF (Снайпер)
    .desc = Облегчённый бронекомплект специалиста MARSOC с боекомплектом к винтовке L112.

ent-STArmorHelmetMARSOC = тактический шлем M3-SOF
    .desc = Бронированный шлем оператора MARSOC со встроенным прибором ночного видения.

ent-STBeltMedicalMarsocFilled = тактический медицинский пояс M276 Сил Специальных Операций
    .desc = Модифицированный пояс боевого медика ССО чёрного цвета, надёжный, как швейцарские часы. Стоп, а Швейцария вообще существует?...

# Жетоны
ent-STDogtagMarsocOfficer = жетоны Офицера MARSOC
    .desc = Засекреченные идентификационные жетоны командования специальных операций.
ent-STDogtagMarsocOfficerHidden = { ent-STDogtagMarsocOfficer }
    .desc = { ent-STDogtagMarsocOfficer.desc }

ent-STDogtagMarsocTeamLeader = жетоны Командира группы MARSOC
    .desc = Засекреченные идентификационные жетоны специальных операций.
ent-STDogtagMarsocTeamLeaderHidden = { ent-STDogtagMarsocTeamLeader }
    .desc = { ent-STDogtagMarsocTeamLeader.desc }

ent-STDogtagMarsocOperative = жетоны Оперативника MARSOC
    .desc = Засекреченные идентификационные жетоны специальных операций.
ent-STDogtagMarsocOperativeHidden = { ent-STDogtagMarsocOperative }
    .desc = { ent-STDogtagMarsocOperative.desc }

ent-STDogtagMarsocMedic = жетоны Медика MARSOC
    .desc = Засекреченные идентификационные жетоны специальных операций.
ent-STDogtagMarsocMedicHidden = { ent-STDogtagMarsocMedic }
    .desc = { ent-STDogtagMarsocMedic.desc }

ent-STDogtagMarsocSpecialist = жетоны Специалиста MARSOC
    .desc = Засекреченные идентификационные жетоны специальных операций.
ent-STDogtagMarsocSpecialistHidden = { ent-STDogtagMarsocSpecialist }
    .desc = { ent-STDogtagMarsocSpecialist.desc }

# Спавнеры
ent-STSpawnerMarsocOfficer = роль призрака офицер MARSOC
    .desc = Спавнер командира операции MARSOC.
ent-STSpawnerMarsocTeamLeader = роль призрака командир группы MARSOC
    .desc = Спавнер младшего лейтенанта MARSOC.
ent-STSpawnerMarsocOperative = роль призрака оперативник MARSOC
    .desc = Спавнер штурмовика MARSOC.
ent-STSpawnerMarsocMedic = роль призрака медик MARSOC
    .desc = Спавнер боевого медика MARSOC.
ent-STSpawnerMarsocSpecialist = роль призрака специалист MARSOC
    .desc = Спавнер снайпера MARSOC с винтовкой L112.

# Действия (Actions)
ent-STActionMarsocDeploySquad = Развернуть отряд MARSOC
    .desc = Активировать протокол пробуждения боевой группы MARSOC на шаттле.

ent-STActionMarsocToggleParadrop = Режим десантирования (Шлюзы)
    .desc = Разблокировать внешние шлюзы шаттла и активировать парашютный сброс на поверхность колонии при шаге в открытый космос.

ent-STActionMarsocConfirmExtraction = Подтвердить эвакуацию (15 сек)
    .desc = Отдать приказ на подъём всех бойцов и целей, находящихся внутри периметра маяка эвакуации.

# Маяк Эвакуации
ent-STMarsocExtractionBeacon = маяк тактической эвакуации «Скайрейнджер»
    .desc = Устанавливает периметр из сигнальных флаеров для экстренной эвакуации группы MARSOC обратно на орбитальный корвет.

ent-STMarsocExtractionFlare = сигнальный флаер эвакуации
    .desc = Обозначает границы зоны подбора отряда MARSOC.

st-marsoc-paradrop-enabled = Внешние шлюзы разблокированы! Прыжок из шлюза десантирует бойцов на поверхность планеты.
st-marsoc-paradrop-disabled = Режим десантирования отключён. Выход в открытый космос смертелен!
st-marsoc-beacon-no-permission = Установить маяк эвакуации может только Офицер, Командир группы, Специалист или Синтетик!
st-marsoc-beacon-deployed = Периметр эвакуации развёрнут! Ожидание подтверждения от Командира...
st-marsoc-beacon-confirmed = Эвакуация подтверждена! Всем занять места внутри периметра флаеров - подъём через 15 секунд!
st-marsoc-beacon-extracted = Группа успешно эвакуирована на борт шаттла MARSOC!
st-marsoc-beacon-none-active = Нет развёрнутого маяка эвакуации!
st-marsoc-beacon-on-shuttle-error = Нельзя развернуть маяк эвакуации на борту самого шаттла!
st-marsoc-beacon-aborted-no-operative = Эвакуация отменена: в периметре маяка нет ни одного живого оперативника MARSOC!

# MARSOC BRIEFING CONSOLE
ent-STComputerMarsocBriefing = терминал специальных директив MARSOC
    .desc = Зашифрованная консоль связи с Высшим Командованием КМП США по Директиве 7-12.

st-marsoc-briefing-window-title = ТЕРМИНАЛ ДИРЕКТИВЫ 7-12 // КАНАЛ СПЕЦОПЕРАЦИЙ MARSOC
st-marsoc-briefing-speaker = КУРАТОР РАЗВЕДКИ КМП США
st-marsoc-briefing-objectives-header = АКТИВНЫЕ ДИРЕКТИВЫ МИССИИ:
st-marsoc-briefing-directive-counter = Профиль директивы: {$current} / {$total}
st-marsoc-briefing-access-denied = Отказано в доступе: требуется уровень допуска Офицера MARSOC.

st-marsoc-briefing-radio-status = [ КАНАЛ СПУТНИКОВОЙ СВЯЗИ: ЗАШИФРОВАНО // ЧАСТОТА 142.85 МГц ]
st-marsoc-briefing-sector-display = Сектор сброса: {$sector}
st-marsoc-briefing-segment-counter = Фрагмент передачи: [ {$current} / {$total} ]

st-marsoc-intel-detected = ПЕРЕХВАТ: активность [{$faction}] (целей: {$count})
st-marsoc-intel-standard = СКАНИРОВАНИЕ ОРБИТЫ: штатный режим

st-marsoc-sector-prefix = Сектор высадки №{$index}
st-marsoc-sector-north = Северный периметр (безопасная зона)
st-marsoc-sector-south = Южный периметр (безопасная зона)
st-marsoc-sector-west = Западный фланг (безопасная зона)
st-marsoc-sector-east = Восточный фланг (безопасная зона)

# Случайные спикеры Командования
st-marsoc-speaker-handler = КУРАТОР ОПЕРАТИВНОГО УПРАВЛЕНИЯ КМП
st-marsoc-speaker-general = КОМАНДУЮЩИЙ СПЕЦОПЕРАЦИЯМИ КМП США
st-marsoc-speaker-director = ДИРЕКТОР СОВЕТА ПО ДИРЕКТИВЕ 7-12
st-marsoc-speaker-random-1 = Генерал-майор София Миллер // Управление специальных операций КМП США
st-marsoc-speaker-random-2 = Полковник София Миллер // Куратор разведки Сектора Неромы
st-marsoc-speaker-random-3 = Бригадный генерал София Миллер // Комитет по Директиве 7-12
st-marsoc-speaker-random-4 = Полковник София Миллер // Штаб 1-го батальона рейдеров КМП

# Кнопки терминала
st-marsoc-briefing-btn-skip = ПРОПУСТИТЬ ТЕКСТ
st-marsoc-briefing-btn-next = СМЕНИТЬ ДИРЕКТИВУ
st-marsoc-briefing-btn-deploy = СПАВН ОТРЯДА
st-marsoc-briefing-btn-deployed = ОТРЯД УЖЕ РАЗВЁРНУТ
st-marsoc-briefing-btn-paradrop-off = ОТКРЫТЬ ШЛЮЗЫ (НАЧАТЬ СБРОС)
st-marsoc-briefing-btn-paradrop-on = БЛОКИРОВАТЬ ШЛЮЗЫ
st-marsoc-briefing-btn-sector = СМЕНИТЬ СЕКТОР СБРОСА
st-marsoc-briefing-btn-lock-sector = ПОДТВЕРДИТЬ СЕКТОР СБРОСА
st-marsoc-briefing-btn-sector-locked = СЕКТОР СБРОСА ЗАФИКСИРОВАН
st-marsoc-briefing-btn-launch = ВЫЛЕТ К ЦЕЛИ
st-marsoc-briefing-btn-in-transit = В ПОЛЁТЕ К ЦЕЛИ ({$seconds} СЕК)
st-marsoc-briefing-btn-arrived = НА ОРБИТЕ СЕКТОРА
st-marsoc-briefing-btn-flight-on = ХОД ШАТТЛА: ОРБИТАЛЬНЫЙ ПОЛЁТ
st-marsoc-briefing-btn-flight-off = ХОД ШАТТЛА: ТИХИЙ ДРЕЙФ
st-marsoc-briefing-btn-broadcast = ПЕРЕДАТЬ ДИРЕКТИВУ ОТРЯДУ

# Всплывающие уведомления терминала
st-marsoc-flight-not-arrived = Дождитесь выхода корвета на орбиту целевого сектора!
st-marsoc-sector-not-locked = Сначала выберите и подтвердите зону сброса!

# Баннер операции (Титры)
st-marsoc-banner-operation-default = OPERATION BLACKOUT-SPEAR
st-marsoc-banner-assignment = 1st Marine Raider Battalion "MARSOC"

# Директивы и Миссии (с {$officer} для подстановки позывного Офицера)
st-marsoc-mission-trijent-title = ОПЕРАЦИЯ «КАЗУС БЕЛЛИ» // ДАМБА ТРИДЖЕНТ
st-marsoc-mission-trijent-dialogue = Внимание, {$officer}. Над колонией Дамба Триджент был сбит борт 2-го взвода 173-й воздушно-десантной роты СПН. Они шли за образцами биооружия Ве-Я. Официально нас здесь нет, {$officer}. Если десантники СПН попадут в руки обычных морпехов с «Алмайера», дипломаты замнут инцидент. Высадитесь, найдите выживших бойцов СПН и ликвидируйте их. Нам нужна эта война.
st-marsoc-mission-trijent-obj-1 = Ликвидировать выживших десантников 173-й роты ВДВ СПН на Дамбе Триджент.
st-marsoc-mission-trijent-obj-2 = Эвакуировать жетоны СПН через периметр маяка «Скайрейнджер».

st-marsoc-mission-lv624-title = ОПЕРАЦИЯ «ЧЁРНЫЙ КУПОЛ» // LV-624
st-marsoc-mission-lv624-dialogue = Исполнительный специалист Ве-Я уровня WYC5 забаррикадировался в корпокуполе LV-624 после обнаружения яиц в северном храме, {$officer}. Из паранойи его охрана расстреляла гражданских колонистов, приняв их за боевиков КОФ. Командование требует рычаг давления на совет директоров Ве-Я. Пробейтесь в купол, {$officer}, и доставьте WYC5 живым на борт корвета.
st-marsoc-mission-lv624-obj-1 = Захватить исполнительного специалиста WYC5 или Связного живым.
st-marsoc-mission-lv624-obj-2 = Доставить цель в периметр флаеров маяка эвакуации и поднять на шаттл.

st-marsoc-mission-hybrisa-title = ОПЕРАЦИЯ «ЧУЖОЙ ФЛАГ» // ГИБРИСА-ПРОСПЕРА
st-marsoc-mission-hybrisa-dialogue = Прямое включение, {$officer}. Ве-Я выкупила военную базу ИТМ на Гибрисе под офис своего супервайзера, но охрана корпоратов проиграла улью в первый же день. Сейчас остатки 21-й Имперской парашютной дивизии пытаются удержать периметр. Изымите секретные данные исследований из лаборатории Ве-Я, а ответственность за саботаж возложите на королевских морпехов ИТМ, {$officer}.
st-marsoc-mission-hybrisa-obj-1 = Изъять носители данных из исследовательского комплекса Гибрисы.
st-marsoc-mission-hybrisa-obj-2 = Зачистить свидетелей корпоративной охраны Ве-Я в секторе.

st-marsoc-mission-almayer-title = ДИРЕКТИВА «ВНУТРЕННИЙ КОНТУР» // УДКТ «АЛМАЙЕР»
st-marsoc-mission-almayer-dialogue = Канал зашифрован, {$officer}. Политическое крыло Командования недовольно фракционной позицией Командующего Офицера «Алмайера». Ваша группа прикомандирована в качестве теневого надзора. Проследите за ходом операции: если Командующий провалит высадку или попадёт в захват улья — вмешайтесь, {$officer}, и проведите принудительную эвакуацию либо отстранение.
st-marsoc-mission-almayer-obj-1 = Обеспечить выполнение директивы фракции в отношении командования «Алмайера».
st-marsoc-mission-almayer-obj-2 = Вернуться на борт шаттла MARSOC до окончания операции.

# Дополнительные генерируемые задачи
st-marsoc-mission-opt-1 = Обеспечить полную секретность присутствия группы перед рядовым составом КМП.
st-marsoc-mission-opt-2 = Сохранить жизнь минимум 80% личного состава отряда до завершения эвакуации.
st-marsoc-mission-opt-3 = Собрать и доставить на борт корвета данные тактической разведки из сектора высадки.


# Точки сброса под открытым небом на Гибрисе (по координатам карты)
st-marsoc-sector-hybrisa-nw-checkpoint = Улицы Колонии — СЗ КПП (25, 189)
st-marsoc-sector-hybrisa-nw-lot = Улицы Колонии — СЗ Автопарк (56, 230)
st-marsoc-sector-hybrisa-ne-streets = Улицы Колонии — СВ Периметр (188, 236)

# Статусы сканера инсертов
st-marsoc-intel-tse-survivors = ПЕРЕХВАТ: Авиабаза ИТМ // гарнизон парашютистов Трёх Солнц
st-marsoc-intel-tse-airbase = ПЕРЕХВАТ: Авиабаза ИТМ // комплекс оставлен персоналом
st-marsoc-intel-clf-spaceport = ПЕРЕХВАТ: Космопорт // укреплённый узел боевиков КОФ
st-marsoc-intel-trijent = ПЕРЕХВАТ: Дамба Триджент // сигнал десанта СПН
st-marsoc-intel-lv624 = ПЕРЕХВАТ: LV-624 // бункер руководителя WYC6

# Динамическая миссия для инсерта clf_spaceport.yml
st-marsoc-mission-clf-spaceport-title = ОПЕРАЦИЯ «ЖЕЛЕЗНЫЙ ПРИЧАЛ» // КОСМОПОРТ КОФ
st-marsoc-mission-clf-spaceport-dialogue = Прямое включение, {$officer}. Боевики Колониального Освободительного Фронта захватили Стыковочный Док №2 космопорта Гибрисы и развернули модифицированные гаусс-турели. В их штабе удерживается спецкейс D18 и ключи шифрования колонии. Высадитесь в уличном секторе, {$officer}, вскройте оборону космопорта и изымите кейс D18.
st-marsoc-mission-clf-spaceport-obj-1 = Зачистить Стыковочный Док №2 космопорта и подавить гаусс-турели боевиков КОФ.
st-marsoc-mission-clf-spaceport-obj-2 = Изъять спецкейс D18 и ключи шифрования колонии из штаба КОФ для эвакуации на шаттл.

# Динамическая миссия для инсерта tse_airbase.yml 
st-marsoc-mission-tse-airbase-title = ОПЕРАЦИЯ «ТИХИЙ АНГАР» // АВИАБАЗА ИТМ
st-marsoc-mission-tse-airbase-dialogue = На связи Командование, {$officer}. Спутниковая разведка зафиксировала, что гарнизон авиабазы Империи Трёх Солнц на Гибрисе экстренно покинул периметр, оставив в диспетчерской бортовой самописец и книгу Имперских кодов аутентификации. Высадитесь под открытым небом у СЗ сектора, {$officer}, проникните в ангар ИТМ и эвакуируйте секретные носители до подхода основных сил КМП.
st-marsoc-mission-tse-airbase-obj-1 = Проникнуть в диспетчерскую и оружейный блок авиабазы ИТМ (TSE Airbase).
st-marsoc-mission-tse-airbase-obj-2 = Забрать Имперские коды аутентификации и бортовой самописец колонии на борт корвета.

# Вторичные динамические задачи от малых инсертов выживших 
st-marsoc-subinsert-meridian-obj = Обследовать укреплённый сектор «Меридиан» и изъять данные с компьютера идентификации.
st-marsoc-subinsert-apartments-obj = Обыскать баррикады в жилых апартаментах и изъять кассетные записи Связного Ве-Я.
st-marsoc-subinsert-bar-obj = Зачистить укреплённый бар колонистов и ликвидировать вооружённых свидетелей.

st-marsoc-beacon-under-roof-error = Маяк эвакуации можно развернуть только под открытым небом!

ent-STMarsocDropZoneBase = маркер высадки MARSOC
    .desc = Точка парашютного десантирования отряда MARSOC.
    .suffix = MARSOC, Высадка

ent-STMarsocDropZoneMarkerAlpha = маркер высадки MARSOC (Сектор 1)
    .desc = { ent-STMarsocDropZoneBase.desc }
    .suffix = MARSOC, Высадка, Сектор 1

ent-STMarsocDropZoneMarkerBravo = маркер высадки MARSOC (Сектор 2)
    .desc = { ent-STMarsocDropZoneBase.desc }
    .suffix = MARSOC, Высадка, Сектор 2

ent-STMarsocDropZoneMarkerCharlie = маркер высадки MARSOC (Сектор 3)
    .desc = { ent-STMarsocDropZoneBase.desc }
    .suffix = MARSOC, Высадка, Сектор 3

ent-STMarsocDropZoneMarkerDelta = маркер высадки MARSOC (Сектор 4)
    .desc = { ent-STMarsocDropZoneBase.desc }
    .suffix = MARSOC, Высадка, Сектор 4

ent-STMarsocBunkerControllerMarker = маркер бункера Ве-Я и спавна Директора
    .desc = Спавнит Супервайзера Ве-Я и управляет таймером ставней бункера.
    .suffix = MARSOC, Бункер Ве-Я

ent-STMarsocLoreReport1 = научный отчет лаборатории №1
    .desc = Документ исследовательского комплекса Ве-Я.
    .suffix = MARSOC, Лор

ent-STMarsocLoreReport2 = научный отчет лаборатории №2
    .desc = Документ исследовательского комплекса Ве-Я.
    .suffix = MARSOC, Лор

ent-STMarsocLoreReport3 = научный отчет лаборатории №3
    .desc = Документ исследовательского комплекса Ве-Я.
    .suffix = MARSOC, Лор

ent-STMarsocLoreReport4 = научный отчет лаборатории №4
    .desc = Документ исследовательского комплекса Ве-Я.
    .suffix = MARSOC, Лор

ent-STMarsocWeYaBlackLedger = черная бухгалтерия Ве-Я
    .desc = Финансовая документация руководства колонии.
    .suffix = MARSOC, Квест

ent-STMarsocWeYaResearchData = данные исследований лаборатории
    .desc = Отчет о работе с образцами в криокапсуле.
    .suffix = MARSOC, Квест

ent-STMarsocExcavationJournal = журнал раскопок
    .desc = Полевой дневник археологов из шахт Гибрисы.
    .suffix = MARSOC, Квест

ent-STMarsocWeYaCommandProtocols = командные протоколы Ве-Я
    .desc = Коды вызова группы быстрого реагирования ЧВК через ретранслятор.
    .suffix = MARSOC, Квест

ent-STMarsocAbominationFlask = колба с биообразцом
    .desc = Хрупкий стеклянный сосуд с живым паразитом внутри.
    .suffix = MARSOC, Квест

st-marsoc-bunker-countdown-started = Запущен протокол разблокировки бункера. Ставни откроются через {$minutes} мин.
st-marsoc-bunker-countdown-active = Протокол разблокировки уже запущен. Осталось {$seconds} сек.
st-marsoc-bunker-shutters-opened = Ставни бункера Ве-Я разблокированы и открыты.

st-marsoc-cryopod-extracted = Вы выгрузили данные исследований из криокапсулы.
st-marsoc-cryopod-already-extracted = Данные с этой криокапсулы уже были скачаны.
st-marsoc-case-dispensed = Кейс хранения открывает секретный отсек с документами.
st-marsoc-protocols-activated = Командные протоколы активированы. Сигнал отправлен на орбиту.

# Тексты бумажек документации
st-marsoc-lore-report-1-text = Запись 1.
st-marsoc-lore-report-2-text = Запись 2.
st-marsoc-lore-report-3-text = Запись 3.
st-marsoc-lore-report-4-text = Запись 4.
st-marsoc-black-ledger-text = Документация черной бухгалтерии.
st-marsoc-research-data-text = Данные исследований из криокапсулы.
st-marsoc-excavation-journal-text = Записи раскопок в пещерах.
st-marsoc-command-protocols-text = Коды вызова ЧВК Ве-Я.

st-marsoc-para-objective-1 = Захватить или ликвидировать Супервайзера Ве-Я в бункере.
st-marsoc-para-objective-2 = Забрать данные исследований с большой криокапсулы в восточной лаборатории.
st-marsoc-para-objective-3 = Вскрыть кейс хранения Ве-Я в бункере и эвакуировать секретные документы.
st-marsoc-para-briefing-chat = Задача вашего подразделения на высадку - {$objective}

# Кому подчиняются роли и что нужно сделать. Вместо затычки nobody в чат
st-marsoc-supervisors-tl = Офицеру группы специальных операций 
st-marsoc-supervisors-squad = Офицеру и Командиру группы специальных операций MARSOC
st-marsoc-supervisors-exec = Совету директоров Вестон-Ямада и особым корпоративным директивам
st-marsoc-supervisors-assistant = Руководителю корпоративного управления и секретным директивам Вестон-Ямада

# Заголовки и задачи для вкладки Персонаж (клавиша C)
st-marsoc-charinfo-header = Директивы группы специальных операций MARSOC
st-marsoc-charinfo-default-obj = Выполнять боевые приказы Офицера операции в секторе высадки.

# Рандомные задачи Супервайзера (выбирается одна при спавне в зависимости от предметов)
st-marsoc-supervisor-obj-ledger = Директива Вестон-Ямада. Основная задача выжить и покинуть колонию. Приоритетная цель найти в бункере и эвакуировать черную бухгалтерию компании.
st-marsoc-supervisor-obj-research = Директива Вестон-Ямада. Основная задача выжить и покинуть колонию. Приоритетная цель снять и эвакуировать данные исследований с большой криокапсулы в восточной лаборатории.
st-marsoc-supervisor-obj-flask = Директива Вестон-Ямада. Основная задача выжить и покинуть колонию. Приоритетная цель забрать из кейса хранения хрупкую колбу с биообразцом и доставить ее целой.
st-marsoc-supervisor-obj-journal = Директива Вестон-Ямада. Основная задача выжить и покинуть колонию. Приоритетная цель раздобыть полевой журнал раскопок и активировать командные протоколы при наличии связи.

# Рандомные задачи Помощника Менеджера (50/50 помощь или устранение)
st-marsoc-assistant-obj-help = Директива Вестон-Ямада. Основная задача выжить. Помогите Руководителю корпоративного управления собрать секретные документы комплекса и обеспечьте его эвакуацию.
st-marsoc-assistant-obj-kill = Секретная директива кадрового отдела Вестон-Ямада. Основная задача выжить. Дождитесь момента, когда Руководитель корпоративного управления соберет документы комплекса, после чего ликвидируйте его и эвакуируйтесь с материалами единолично для получения повышения.

department-STMarsoc = Силы специальных операций КМП (MARSOC)
department-STMarsoc-description = Засекреченный контингент 1-го батальона рейдеров КМП США, подчиняющийся напрямую Директиве 7-12.

st-marsoc-exposure-console-status = Уровень обнаружения отряда {$percent}%
st-marsoc-exposure-los-warning = Внимание! Морпехи наблюдают за вами. Немедленно разорвите зрительный контакт!
st-marsoc-exposure-reason-operative-dead = Гибель оперативника MARSOC в секторе
st-marsoc-exposure-reason-marine-killed = Ликвидация морского пехотинца ККМП
st-marsoc-exposure-squad-alert = ТРЕВОГА СЕКРЕТНОСТИ ({$reason}). Уровень обнаружения отряда вырос до {$percent}%!
st-marsoc-exposure-ares-75-warning = Внимание всем подразделениям. Зафиксировано присутствие неизвестной вооруженной группы без транспондеров ККМП в зоне проведения операции.
st-marsoc-exposure-squad-75-warning = КРИТИЧЕСКАЯ УГРОЗА! Уровень обнаружения превысил 75%. АРЕС запеленговал активность группы в секторе!
st-marsoc-exposure-ares-100-compromised = Экстренное оповещение АРЕС. Батареи ПВО Алмайера обнаружили и поразили неопознанный корвет на низкой орбите. Корпорации Вестон-Ямада направлен подробный отчёт о произошедшем, а так же полученые даные.
st-marsoc-exposure-squad-100-compromised = ПРОВАЛ МАСКИРОВКИ (100%). Орбитальный корвет поражен огнем ПВО Алмайера и экстренно покинул орбиту! Маяки эвакуации отключены!