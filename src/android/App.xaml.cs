using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает функционал приложения
	/// </summary>
	public partial class App: Application
		{
		#region Настройки стилей отображения

		private Color aboutMasterBackColor = Color.FromArgb ("#F0FFF0");
		private Color aboutFieldBackColor = Color.FromArgb ("#D0FFD0");
		private Color templateMasterBackColor = Color.FromArgb ("#FFFFF0");
		private Color templateFieldBackColor = Color.FromArgb ("#FFFFD0");
		private Color settingsMasterBackColor = Color.FromArgb ("#F0FFFF");
		private Color settingsFieldBackColor = Color.FromArgb ("#D0FFFF");

		#endregion

		#region Переменные страниц

		private ContentPage templatePage, settingsPage, aboutPage;

		private Button blankTypeButton, kktModelButton, fnModelButton, ofdVariantButton,
			ofdNameButton, addressRegionCodeButton, fnCloseDateButton, fnOpenDateButton,
			findPostIndexButton, createBlankButton;

		private Editor userNameField, innField, ogrnField, kppField, presenterTypeField,
			kktSerialField, fnSerialField, kktRNMField, addressIndexField, addressAreaField,
			addressCityField, addressTownField, addressStreetField, addressHouseField,
			addressBuildingField, addressAppartmentField, placeField, automatNumberField,
			fnCloseFDField, fnCloseFPDField, fnOpenFDField, fnOpenFPDField;

		private Switch fnChangeFlag, presenterTypeFlag, userNameChangeFlag, kktStolenFlag,
			kktMissingFlag, fnBrokenFlag, addressPlaceChangeFlag, exciseFlag, markFlag,
			internetFlag, bsoFlag, bankAgentFlag, agentFlag, deliveryFlag, lotteryFlag,
			gamblingFlag, gamblingExchangeFlag, otherChangeFlag, automatFlag,
			automatAddressIsSameFlag, automatChangeFlag, dontAddStrikeoutsFlag,
			addSignDateFlag, switchHeightFlag;

		private Label fontSizeField, kktStolenLabel, kktMissingLabel, fnBrokenLabel,
			userNameChangeLabel, fnChangeLabel, kktRNMLabel, addressPlaceChangeLabel,
			automatChangeLabel, otherChangeLabel, fnModelLabel, fnSerialLabel,
			ofdVariantLabel, ofdNameLabel, addressRegionCodeLabel, addressAreaLabel,
			addressIndexLabel, addressCityLabel, addressTownLabel, addressStreetLabel,
			addressHouseLabel, addressBuildingLabel, addressAppartmentLabel, placeLabel,
			lotteryLabel, gamblingLabel, gamblingExchangeLabel, bankAgentLabel, agentLabel,
			deliveryLabel, markLabel, exciseLabel, internetLabel, bsoLabel, automatLabel,
			fnCloseFDLabel1, fnCloseFDLabel2, fnCloseDateLabel, fnCloseFPDLabel, fnOpenFDLabel1,
			fnOpenFDLabel2, fnOpenDateLabel, fnOpenFPDLabel, automatNumberLabel, automatAddressIsSameLabel;

		private ScrollView mainField;

		#endregion

		#region Основные переменные

		// Опорные классы
		private KnowledgeBase kb;

		// Список вариантов меню
		private List<string> blankTypeVariants = [];
		private List<string> kktModelVariants = [];
		private List<string> fnModelVariants = [];
		private List<string> ofdNameVariants = [];
		private List<string> addressRegionCodeVariants = [];
		private List<string> menuVariants = [];

		// Выбранный тип заявления
		private BlankTypes blankType = BlankTypes.RegistrationChange;
		private int kktModel = 0;
		private int fnModel = 0;
		private OFDVariants ofdVariant = OFDVariants.ContinueWithOFD;
		private int ofdName = 0;
		private int addressRegionCode = (int)KAPRSupport.RegionIndex;

		private DateTime fnCloseDate = LibrarySupport.MinimumDatePickerValue;
		private DateTime fnOpenDate = LibrarySupport.MinimumDatePickerValue;
		private static char[] dateSplitters = [' ', '/', '.', ',', ':', '-'];

		#endregion

		#region Запуск и настройка

		/// <summary>
		/// Конструктор. Точка входа приложения
		/// </summary>
		public App ()
			{
			// Инициализация
			InitializeComponent ();
			}

		// Замена определению MainPage = new MasterPage ()
		protected override Window CreateWindow (IActivationState activationState)
			{
			return new Window (AppShell ());
			}

		// Инициализация интерфейса
		private Page AppShell ()
			{
			Page mainPage = new MasterPage ();
			RDAppStartupFlags flags = RDGenerics.GetAppStartupFlags (RDAppStartupFlags.DisableXPUN);

			kb = new KnowledgeBase ();

			#region Общая конструкция страниц приложения

			templatePage = RDInterface.ApplyPageSettings (new TemplatePage (),
				"Заполнение заявления", templateMasterBackColor);
			settingsPage = RDInterface.ApplyPageSettings (new SettingsPage (),
				"Настройки", settingsMasterBackColor);
			aboutPage = RDInterface.ApplyPageSettings (new AboutPage (),
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout),
				aboutMasterBackColor);

			RDInterface.SetMasterPage (mainPage, templatePage, templateMasterBackColor);
			DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;
			RDInterface.MasterPage.Popped += Current_LogPagePopped;

			#endregion

			#region Основные параметры заявления

			RDButtonFlags bf = RDButtonFlags.BiggerFontSize | RDButtonFlags.EnableShadow;

			Label h1 = RDInterface.ApplyLabelSettings (templatePage, "GenericFieldsLabel", "Основные параметры заявления",
				RDLabelTypes.HeaderCenter);
			AlignHeader (h1);

			RDInterface.ApplyLabelSettings (templatePage, "BlankTypeLabel", "Тип заявления:",
				RDLabelTypes.DefaultLeft);
			blankTypeButton = RDInterface.ApplyButtonSettings (templatePage, "BlankTypeButton",
				" ", templateFieldBackColor, BlankTypeButton_Clicked, bf);

			fnChangeLabel = RDInterface.ApplyLabelSettings (templatePage, "FNChangeLabel", "Выполняется замена ФН",
				RDLabelTypes.DefaultLeft);
			fnChangeFlag = RDInterface.ApplySwitchSettings (templatePage, "FNChangeFlag", false,
				templateFieldBackColor, FNChangeFlag_CheckedChanged, false);

			RDInterface.ApplyLabelSettings (templatePage, "UserNameLabel", "Наименование пользователя ККТ:",
				RDLabelTypes.DefaultLeft);
			userNameField = RDInterface.ApplyEditorSettings (templatePage, "UserNameField", templateFieldBackColor,
				Keyboard.Text, 120, "", null, true);
			AlignEditor (userNameField, true, false);

			RDInterface.ApplyLabelSettings (templatePage, "INNLabel", "ИНН:", RDLabelTypes.DefaultLeft);
			innField = RDInterface.ApplyEditorSettings (templatePage, "INNField", templateFieldBackColor,
				Keyboard.Numeric, KAPRSupport.GetFieldLength (BlankFields.INN), "", null, true);
			AlignEditor (innField, false, true);

			RDInterface.ApplyLabelSettings (templatePage, "OGRNLabel", "ОГРН:", RDLabelTypes.DefaultLeft);
			ogrnField = RDInterface.ApplyEditorSettings (templatePage, "OGRNField", templateFieldBackColor,
				Keyboard.Numeric, KAPRSupport.GetFieldLength (BlankFields.OGRN), "", null, true);
			AlignEditor (ogrnField, false, true);

			RDInterface.ApplyLabelSettings (templatePage, "KPPLabel", "КПП:", RDLabelTypes.DefaultLeft);
			kppField = RDInterface.ApplyEditorSettings (templatePage, "KPPField", templateFieldBackColor,
				Keyboard.Numeric, KAPRSupport.GetFieldLength (BlankFields.KPP), "", null, true);
			AlignEditor (kppField, false, true);

			RDInterface.ApplyButtonSettings (templatePage, "FindUserButton", "Найти", templateFieldBackColor,
				FindUserButton_Click);

			RDInterface.ApplyLabelSettings (templatePage, "PresenterTypeLabel", "ФИО заявителя:",
				RDLabelTypes.DefaultLeft);
			presenterTypeField = RDInterface.ApplyEditorSettings (templatePage, "PresenterTypeField", templateFieldBackColor,
				Keyboard.Text, 60, "", null, true);
			AlignEditor (presenterTypeField, true, false);

			RDInterface.ApplyLabelSettings (templatePage, "PresenterTypeFlagLabel", "По доверенности",
				RDLabelTypes.DefaultLeft);
			presenterTypeFlag = RDInterface.ApplySwitchSettings (templatePage, "PresenterTypeFlag", false,
				templateFieldBackColor, null, false);

			userNameChangeLabel = RDInterface.ApplyLabelSettings (templatePage, "UserNameChangeLabel", "Наименование пользователя ККТ\nизменилось",
				RDLabelTypes.DefaultLeft);
			userNameChangeFlag = RDInterface.ApplySwitchSettings (templatePage, "UserNameChangeFlag", false,
				templateFieldBackColor, null, false);

			kktStolenLabel = RDInterface.ApplyLabelSettings (templatePage, "KKTStolenLabel", "ККТ похищена",
				RDLabelTypes.DefaultLeft);
			kktStolenFlag = RDInterface.ApplySwitchSettings (templatePage, "KKTStolenFlag", false,
				templateFieldBackColor, null, false);

			kktMissingLabel = RDInterface.ApplyLabelSettings (templatePage, "KKTMissingLabel", "ККТ утеряна",
				RDLabelTypes.DefaultLeft);
			kktMissingFlag = RDInterface.ApplySwitchSettings (templatePage, "KKTMissingFlag", false,
				templateFieldBackColor, null, false);

			fnBrokenLabel = RDInterface.ApplyLabelSettings (templatePage, "FNBrokenLabel", "ФН неисправен",
				RDLabelTypes.DefaultLeft);
			fnBrokenFlag = RDInterface.ApplySwitchSettings (templatePage, "FNBrokenFlag", false,
				templateFieldBackColor, FNBrokenFlag_CheckedChanged, false);

			#endregion

			#region ЗН ККТ, ЗН ФН, РНМ, ОФД

			Label h2 = RDInterface.ApplyLabelSettings (templatePage, "NumberFieldsLabel", "ККТ, ФН, ОФД, РНМ",
				RDLabelTypes.HeaderCenter);
			AlignHeader (h2);

			RDInterface.ApplyLabelSettings (templatePage, "KKTModelLabel", "Модель ККТ:",
				RDLabelTypes.DefaultLeft);
			kktModelButton = RDInterface.ApplyButtonSettings (templatePage, "KKTModelButton",
				" ", templateFieldBackColor, KKTModelButton_Clicked, bf);

			RDInterface.ApplyLabelSettings (templatePage, "KKTSerialLabel", "ЗН ККТ:",
				RDLabelTypes.DefaultLeft);
			kktSerialField = RDInterface.ApplyEditorSettings (templatePage, "KKTSerialField", templateFieldBackColor,
				Keyboard.Numeric, KAPRSupport.GetFieldLength (BlankFields.KKTSerialNumber_Line1), "",
				KKTSerialField_TextChanged, true);
			AlignEditor (kktSerialField, true, true);

			fnModelLabel = RDInterface.ApplyLabelSettings (templatePage, "FNModelLabel", "Модель ФН:",
				RDLabelTypes.DefaultLeft);
			fnModelButton = RDInterface.ApplyButtonSettings (templatePage, "FNModelButton",
				" ", templateFieldBackColor, FNModelButton_Clicked, bf);

			fnSerialLabel = RDInterface.ApplyLabelSettings (templatePage, "FNSerialLabel", "ЗН ФН:",
				RDLabelTypes.DefaultLeft);
			fnSerialField = RDInterface.ApplyEditorSettings (templatePage, "FNSerialField", templateFieldBackColor,
				Keyboard.Numeric, 16, "", FNSerialField_TextChanged, true);
			AlignEditor (fnSerialField, true, true);

			ofdVariantLabel = RDInterface.ApplyLabelSettings (templatePage, "OFDVariantLabel", "Режим работы с ОФД:",
				RDLabelTypes.DefaultLeft);
			ofdVariantButton = RDInterface.ApplyButtonSettings (templatePage, "OFDVariantButton",
				" ", templateFieldBackColor, OFDVariantButton_Clicked, bf);

			ofdNameLabel = RDInterface.ApplyLabelSettings (templatePage, "OFDNameLabel", "Название ОФД:",
				RDLabelTypes.DefaultLeft);
			ofdNameButton = RDInterface.ApplyButtonSettings (templatePage, "OFDNameButton",
				" ", templateFieldBackColor, OFDNameButton_Clicked, bf);

			kktRNMLabel = RDInterface.ApplyLabelSettings (templatePage, "KKTRNMLabel", "Регистрационный номер ККТ:",
				RDLabelTypes.DefaultLeft);
			kktRNMField = RDInterface.ApplyEditorSettings (templatePage, "KKTRNMField", templateFieldBackColor,
				Keyboard.Numeric, 16, "", null, true);
			AlignEditor (kktRNMField, true, true);

			#endregion

			#region Адрес и место расчётов

			Label h3 = RDInterface.ApplyLabelSettings (templatePage, "AddressFieldsLabel", "Адрес и место расчётов",
				RDLabelTypes.HeaderCenter);
			AlignHeader (h3);

			addressRegionCodeLabel = RDInterface.ApplyLabelSettings (templatePage, "AddressRegionCodeLabel", "Регион РФ:",
				RDLabelTypes.DefaultLeft);
			addressRegionCodeButton = RDInterface.ApplyButtonSettings (templatePage, "AddressRegionCodeButton",
				" ", templateFieldBackColor, AddressRegionCodeButton_Clicked, bf);

			addressIndexLabel = RDInterface.ApplyLabelSettings (templatePage, "AddressIndexLabel", "Индекс:",
				RDLabelTypes.DefaultLeft);
			addressIndexField = RDInterface.ApplyEditorSettings (templatePage, "AddressIndexField", templateFieldBackColor,
				Keyboard.Numeric, KAPRSupport.GetFieldLength (BlankFields.UserAddressIndex), "", null, true);
			AlignEditor (addressIndexField, false, true);

			findPostIndexButton = RDInterface.ApplyButtonSettings (templatePage, "AddressIndexFindButton",
				"Найти", templateFieldBackColor, FindPostIndex_Click);

			addressAreaLabel = RDInterface.ApplyLabelSettings (templatePage, "AddressAreaLabel", "Район:",
				RDLabelTypes.DefaultLeft);
			addressAreaField = RDInterface.ApplyEditorSettings (templatePage, "AddressAreaField", templateFieldBackColor,
				Keyboard.Text, KAPRSupport.GetFieldLength (BlankFields.UserAddressArea), "", null, true);
			AlignEditor (addressAreaField, true, false);

			addressCityLabel = RDInterface.ApplyLabelSettings (templatePage, "AddressCityLabel", "Город:",
				RDLabelTypes.DefaultLeft);
			addressCityField = RDInterface.ApplyEditorSettings (templatePage, "AddressCityField", templateFieldBackColor,
				Keyboard.Text, KAPRSupport.GetFieldLength (BlankFields.UserAddressCity), "", null, true);
			AlignEditor (addressCityField, true, false);

			addressTownLabel = RDInterface.ApplyLabelSettings (templatePage, "AddressTownLabel", "Населённый пункт:",
				RDLabelTypes.DefaultLeft);
			addressTownField = RDInterface.ApplyEditorSettings (templatePage, "AddressTownField", templateFieldBackColor,
				Keyboard.Text, KAPRSupport.GetFieldLength (BlankFields.UserAddressTown), "", null, true);
			AlignEditor (addressTownField, true, false);

			addressStreetLabel = RDInterface.ApplyLabelSettings (templatePage, "AddressStreetLabel", "Улица:",
				RDLabelTypes.DefaultLeft);
			addressStreetField = RDInterface.ApplyEditorSettings (templatePage, "AddressStreetField", templateFieldBackColor,
				Keyboard.Text, KAPRSupport.GetFieldLength (BlankFields.UserAddressStreet), "", null, true);
			AlignEditor (addressStreetField, true, false);

			addressHouseLabel = RDInterface.ApplyLabelSettings (templatePage, "AddressHouseLabel", "Дом / владение:",
				RDLabelTypes.DefaultLeft);
			addressHouseField = RDInterface.ApplyEditorSettings (templatePage, "AddressHouseField", templateFieldBackColor,
				Keyboard.Text, KAPRSupport.GetFieldLength (BlankFields.UserAddressHouseNumber), "", null, true);

			addressBuildingLabel = RDInterface.ApplyLabelSettings (templatePage, "AddressBuildingLabel", "Корпус / строение:",
				RDLabelTypes.DefaultLeft);
			addressBuildingField = RDInterface.ApplyEditorSettings (templatePage, "AddressBuildingField", templateFieldBackColor,
				Keyboard.Text, KAPRSupport.GetFieldLength (BlankFields.UserAddressBuildingNumber), "", null, true);

			addressAppartmentLabel = RDInterface.ApplyLabelSettings (templatePage, "AddressAppartmentLabel", "Квартира / помещение:",
				RDLabelTypes.DefaultLeft);
			addressAppartmentField = RDInterface.ApplyEditorSettings (templatePage, "AddressAppartmentField", templateFieldBackColor,
				Keyboard.Text, KAPRSupport.GetFieldLength (BlankFields.UserAddressAppartmentNumber), "", null, true);

			placeLabel = RDInterface.ApplyLabelSettings (templatePage, "PlaceLabel", "Место расчётов:",
				RDLabelTypes.DefaultLeft);
			placeField = RDInterface.ApplyEditorSettings (templatePage, "PlaceField", templateFieldBackColor,
				Keyboard.Text, 60, "", null, true);
			AlignEditor (placeField, true, false);

			addressPlaceChangeLabel = RDInterface.ApplyLabelSettings (templatePage, "AddressPlaceChangeLabel", "Адрес и / или место расчётов изменились",
				RDLabelTypes.DefaultLeft);
			addressPlaceChangeFlag = RDInterface.ApplySwitchSettings (templatePage, "AddressPlaceChangeFlag", false,
				templateFieldBackColor, null, false);

			#endregion

			#region Режимы работы

			Label h4 = RDInterface.ApplyLabelSettings (templatePage, "WorkmodeFieldsLabel", "Режимы работы",
				RDLabelTypes.HeaderCenter);
			AlignHeader (h4);

			exciseLabel = RDInterface.ApplyLabelSettings (templatePage, "ExciseLabel", "Продажа подакцизного товара",
				RDLabelTypes.DefaultLeft);
			exciseFlag = RDInterface.ApplySwitchSettings (templatePage, "ExciseFlag", false,
				templateFieldBackColor, null, false);

			markLabel = RDInterface.ApplyLabelSettings (templatePage, "MarkLabel", "Продажа маркированного товара",
				RDLabelTypes.DefaultLeft);
			markFlag = RDInterface.ApplySwitchSettings (templatePage, "MarkFlag", false,
				templateFieldBackColor, null, false);

			internetLabel = RDInterface.ApplyLabelSettings (templatePage, "InternetLabel", "Работа в сети интернет",
				RDLabelTypes.DefaultLeft);
			internetFlag = RDInterface.ApplySwitchSettings (templatePage, "InternetFlag", false,
				templateFieldBackColor, null, false);

			bsoLabel = RDInterface.ApplyLabelSettings (templatePage, "BSOLabel", "Режим бланков строгой отчётности",
				RDLabelTypes.DefaultLeft);
			bsoFlag = RDInterface.ApplySwitchSettings (templatePage, "BSOFlag", false,
				templateFieldBackColor, null, false);

			bankAgentLabel = RDInterface.ApplyLabelSettings (templatePage, "BankAgentLabel", "Банковский платёжный агент",
				RDLabelTypes.DefaultLeft);
			bankAgentFlag = RDInterface.ApplySwitchSettings (templatePage, "BankAgentFlag", false,
				templateFieldBackColor, null, false);

			agentLabel = RDInterface.ApplyLabelSettings (templatePage, "AgentLabel", "Платёжный агент",
				RDLabelTypes.DefaultLeft);
			agentFlag = RDInterface.ApplySwitchSettings (templatePage, "AgentFlag", false,
				templateFieldBackColor, null, false);

			deliveryLabel = RDInterface.ApplyLabelSettings (templatePage, "DeliveryLabel", "Развозная / разносная торговля",
				RDLabelTypes.DefaultLeft);
			deliveryFlag = RDInterface.ApplySwitchSettings (templatePage, "DeliveryFlag", false,
				templateFieldBackColor, null, false);

			lotteryLabel = RDInterface.ApplyLabelSettings (templatePage, "LotteryLabel", "Проведение лотерей",
				RDLabelTypes.DefaultLeft);
			lotteryFlag = RDInterface.ApplySwitchSettings (templatePage, "LotteryFlag", false,
				templateFieldBackColor, null, false);

			gamblingLabel = RDInterface.ApplyLabelSettings (templatePage, "GamblingLabel", "Проведение азартных игр",
				RDLabelTypes.DefaultLeft);
			gamblingFlag = RDInterface.ApplySwitchSettings (templatePage, "GamblingFlag", false,
				templateFieldBackColor, null, false);

			gamblingExchangeLabel = RDInterface.ApplyLabelSettings (templatePage, "GamblingExchangeLabel",
				"Работа с обменными знаками казино", RDLabelTypes.DefaultLeft);
			gamblingExchangeFlag = RDInterface.ApplySwitchSettings (templatePage, "GamblingExchangeFlag", false,
				templateFieldBackColor, null, false);

			otherChangeLabel = RDInterface.ApplyLabelSettings (templatePage, "OtherChangeLabel", "Режим работы ККТ изменился",
				RDLabelTypes.DefaultLeft);
			otherChangeFlag = RDInterface.ApplySwitchSettings (templatePage, "OtherChangeFlag", false,
				templateFieldBackColor, null, false);

			automatLabel = RDInterface.ApplyLabelSettings (templatePage, "AutomatLabel", "Автоматический режим",
				RDLabelTypes.DefaultLeft);
			automatFlag = RDInterface.ApplySwitchSettings (templatePage, "AutomatFlag", false,
				templateFieldBackColor, AutomatFlag_CheckedChanged, false);

			automatNumberLabel = RDInterface.ApplyLabelSettings (templatePage, "AutomatNumberLabel", "Номер автомата:",
				RDLabelTypes.DefaultLeft);
			automatNumberField = RDInterface.ApplyEditorSettings (templatePage, "AutomatNumberField", templateFieldBackColor,
				Keyboard.Numeric, KAPRSupport.GetFieldLength (BlankFields.AutomatNumber), "", null, true);
			AlignEditor (automatNumberField, false, true);

			automatAddressIsSameLabel = RDInterface.ApplyLabelSettings (templatePage, "AutomatAddressIsSameLabel",
				"Адрес и место расположения автомата\nи ККТ совпадают", RDLabelTypes.DefaultLeft);
			automatAddressIsSameFlag = RDInterface.ApplySwitchSettings (templatePage, "AutomatAddressIsSameFlag", false,
				templateFieldBackColor, null, false);

			automatChangeLabel = RDInterface.ApplyLabelSettings (templatePage, "AutomatChangeLabel", "Сведения (включая адрес) об автомате\nизменились",
				RDLabelTypes.DefaultLeft);
			automatChangeFlag = RDInterface.ApplySwitchSettings (templatePage, "AutomatChangeFlag", false,
				templateFieldBackColor, AutomatFlag_CheckedChanged, false);

			#endregion

			#region Реквизиты отчётов

			Label h5 = RDInterface.ApplyLabelSettings (templatePage, "ReportsFieldsLabel", "Реквизиты отчётов ФН",
				RDLabelTypes.HeaderCenter);
			AlignHeader (h5);

			fnCloseFDLabel1 = RDInterface.ApplyLabelSettings (templatePage, "FNCloseFDLabel1",
				"Отчёт о закрытии ФН", RDLabelTypes.HeaderLeft);

			fnCloseFDLabel2 = RDInterface.ApplyLabelSettings (templatePage, "FNCloseFDLabel2",
				"Номер документа:", RDLabelTypes.DefaultLeft);
			fnCloseFDField = RDInterface.ApplyEditorSettings (templatePage, "FNCloseFDField", templateFieldBackColor,
				Keyboard.Numeric, 6, "", null, true);
			AlignEditor (fnCloseFDField, true, true);

			fnCloseDateLabel = RDInterface.ApplyLabelSettings (templatePage, "FNCloseDateLabel", "Дата формирования:",
				RDLabelTypes.DefaultLeft);
			fnCloseDateButton = RDInterface.ApplyButtonSettings (templatePage, "FNCloseDateButton",
				" ", templateFieldBackColor, FNDateButton_Clicked, bf);
			fnCloseDateButton.FontFamily = RDGenerics.MonospaceFont;

			fnCloseFPDLabel = RDInterface.ApplyLabelSettings (templatePage, "FNCloseFPDLabel", "Фискальный признак:",
				RDLabelTypes.DefaultLeft);
			fnCloseFPDField = RDInterface.ApplyEditorSettings (templatePage, "FNCloseFPDField", templateFieldBackColor,
				Keyboard.Numeric, KAPRSupport.GetFieldLength (BlankFields.FNCloseDocumentSign), "", null, true);
			AlignEditor (fnCloseFPDField, true, true);

			fnOpenFDLabel1 = RDInterface.ApplyLabelSettings (templatePage, "FNOpenFDLabel1",
				"Отчёт о регистрации / перерегистрации ФН", RDLabelTypes.HeaderLeft);

			fnOpenFDLabel2 = RDInterface.ApplyLabelSettings (templatePage, "FNOpenFDLabel2",
				"Номер документа:", RDLabelTypes.DefaultLeft);

			fnOpenFDField = RDInterface.ApplyEditorSettings (templatePage, "FNOpenFDField", templateFieldBackColor,
				Keyboard.Numeric, 6, "", null, true);
			AlignEditor (fnOpenFDField, true, true);

			fnOpenDateLabel = RDInterface.ApplyLabelSettings (templatePage, "FNOpenDateLabel", "Дата формирования:",
				RDLabelTypes.DefaultLeft);
			fnOpenDateButton = RDInterface.ApplyButtonSettings (templatePage, "FNOpenDateButton",
				" ", templateFieldBackColor, FNDateButton_Clicked, bf);
			fnOpenDateButton.FontFamily = RDGenerics.MonospaceFont;

			fnOpenFPDLabel = RDInterface.ApplyLabelSettings (templatePage, "FNOpenFPDLabel", "Фискальный признак:",
				RDLabelTypes.DefaultLeft);
			fnOpenFPDField = RDInterface.ApplyEditorSettings (templatePage, "FNOpenFPDField", templateFieldBackColor,
				Keyboard.Numeric, KAPRSupport.GetFieldLength (BlankFields.FNOpenDocumentSign), "", null, true);
			AlignEditor (fnOpenFPDField, true, true);

			#endregion

			// Применение первичных настроек только после инициализации всех полей
			KKTModelButton_Clicked (null, null);
			FNModelButton_Clicked (null, null);
			OFDNameButton_Clicked (null, null);
			AddressRegionCodeButton_Clicked (null, null);
			AutomatFlag_CheckedChanged (null, null);
			FNDateButton_Clicked (fnCloseDateButton, null);
			FNDateButton_Clicked (fnOpenDateButton, null);

			BlankTypeButton_Clicked (null, null);   // Строго последний

			#region Прочие функции

			createBlankButton = RDInterface.ApplyButtonSettings (templatePage, "CreateBlankButton", "Сформировать заявление",
				templateFieldBackColor, CreateBlank_Click, RDButtonFlags.BiggerFontSize);
			createBlankButton.TextColor = h1.TextColor;
			createBlankButton.BackgroundColor = RDInterface.GetInterfaceColor (RDInterfaceColors.SuccessText);
			createBlankButton.FontAttributes = FontAttributes.Bold;

			Button mn = RDInterface.ApplyButtonSettings (templatePage, "MenuButton", RDDefaultButtons.Menu,
				templateFieldBackColor, MenuButton_Click, true);
			createBlankButton.HeightRequest = mn.HeightRequest;

			mainField = (ScrollView)templatePage.FindByName ("MainField");

			#endregion

			#region Страница "О программе"

			RDInterface.ApplyLabelSettings (aboutPage, "AboutLabel",
				RDGenerics.AppAboutLabelText, RDLabelTypes.AppAbout);

			RDInterface.ApplyButtonSettings (aboutPage, "ManualsButton",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_ReferenceMaterials),
				aboutFieldBackColor, ReferenceButton_Click);
			RDInterface.ApplyButtonSettings (aboutPage, "HelpButton",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_HelpSupport),
				aboutFieldBackColor, HelpButton_Click);

			#endregion

			#region Настройки приложения

			RDInterface.ApplyLabelSettings (settingsPage, "DontAddStrikeoutsLabel",
				"Не добавлять прочерки в незаполненные\nполя", RDLabelTypes.DefaultLeft);
			dontAddStrikeoutsFlag = RDInterface.ApplySwitchSettings (settingsPage, "DontAddStrikeoutsFlag", false,
				settingsFieldBackColor, DontAddStrikeouts_Toggled, KAPRSupport.DontAddStrikeouts);
			RDInterface.ApplyLabelSettings (settingsPage, "DontAddStrikeoutsTip",
				"По умолчанию сформированное заявление требует заполнения пустых ячеек прочерками. В случае, " +
				"если часть из них после печати требуется заполнить вручную, следует включить эту опцию",
				RDLabelTypes.TipJustify);

			RDInterface.ApplyLabelSettings (settingsPage, "AddSignDateLabel",
				"Подставлять дату подачи заявления рядом\nс полем подписи (не рекомендуется)", RDLabelTypes.DefaultLeft);
			addSignDateFlag = RDInterface.ApplySwitchSettings (settingsPage, "AddSignDateFlag", false,
				settingsFieldBackColor, AddSignDate_Toggled, KAPRSupport.AddSignDate);
			RDInterface.ApplyLabelSettings (settingsPage, "AddSignDateTip",
				"По умолчанию шаблон заявления не предполагает автоматической подстановки текущей даты рядом " +
				"со строкой подписи пользователя ККТ. Однако в некоторых случаях это может ускорить процесс " +
				"оформления документов", RDLabelTypes.TipJustify);

			RDInterface.ApplyLabelSettings (settingsPage, "SwitchHeightLabel",
				"Прижать панель кнопок к низу экрана\n(раздвинуть область прокрутки)", RDLabelTypes.DefaultLeft);
			switchHeightFlag = RDInterface.ApplySwitchSettings (settingsPage, "SwitchHeightFlag", false,
				settingsFieldBackColor, SwitchHeightFlag_Toggled, KAPRSupport.AdditionalHeight != 0);
			RDInterface.ApplyLabelSettings (settingsPage, "SwitchHeightTip",
				"На некоторых устройствах расположение системной панели «Назад-Свернуть-Перейти» позволяет " +
				"немного увеличить область прокрутки полей заявления в интерфейсе приложения. Не включайте эту " +
				"функцию, если кнопка «Сформировать» плотно прилегает к этой панели", RDLabelTypes.TipJustify);

			RDInterface.ApplyLabelSettings (settingsPage, "RestartTipLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Message_RestartRequired),
				RDLabelTypes.TipCenter);

			RDInterface.ApplyLabelSettings (settingsPage, "FontSizeLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_InterfaceFontSize),
				RDLabelTypes.DefaultLeft);
			RDInterface.ApplyButtonSettings (settingsPage, "FontSizeInc",
				RDDefaultButtons.Increase, settingsFieldBackColor, FontSizeButton_Clicked, true);
			RDInterface.ApplyButtonSettings (settingsPage, "FontSizeDec",
				RDDefaultButtons.Decrease, settingsFieldBackColor, FontSizeButton_Clicked, true);
			fontSizeField = RDInterface.ApplyLabelSettings (settingsPage, "FontSizeField",
				" ", RDLabelTypes.DefaultCenter);

			RDInterface.ApplyLabelSettings (settingsPage, "FontSizeTipLabel",
				"Размер шрифта интерфейса влияет на все элементы в приложении. Измените его, если " +
				"автоматическое масштабирование не дало желаемого результата", RDLabelTypes.TipJustify);
			FontSizeButton_Clicked (null, null);

			#endregion

			// Обязательное принятие Политики и EULA
			AcceptPolicy (flags.HasFlag (RDAppStartupFlags.DisableXPUN));
			return mainPage;
			}

		private static void AlignEditor (Editor TB, bool Center, bool Monospace)
			{
			if (Center)
				{
				TB.HorizontalOptions = LayoutOptions.Fill;
				TB.HorizontalTextAlignment = TextAlignment.Center;
				}

			if (Monospace)
				TB.FontFamily = RDGenerics.MonospaceFont;
			}

		private void AlignHeader (Label LB)
			{
			LB.FontSize *= 1.25f;
			LB.Margin = Thickness.Zero;
			LB.Padding = new Thickness (6);
			LB.TextColor = templateMasterBackColor;
			LB.BackgroundColor = RDInterface.GetInterfaceColor (RDInterfaceColors.DefaultText);
			LB.HorizontalOptions = LayoutOptions.Fill;
			}

		// Контроль принятия Политики и EULA
		private static async void AcceptPolicy (bool DisableXPUN)
			{
			// Контроль XPUN
			if (!DisableXPUN)
				await RDInterface.XPUNLoop ();

			// Политика
			if (RDGenerics.TipsState != 0)
				return;

			await RDInterface.PolicyLoop ();

			// Только после принятия
			await RDInterface.ShowMessage ("Вас приветствует " + ProgramDescription.AssemblyMainName +
				"PR – " + ProgramDescription.AssemblyDescription + RDLocale.RNRN +
				"Данный инструмент позволяет формировать бумажные заявления согласно формам по КНД 1110061 и 1110062. " +
				"На этой странице находится перечень полей, входящих в состав заявления выбранного типа. " +
				"Любое из них можно оставить пустым – в этом случае поля можно будет заполнить вручную " +
				"в распечатанном документе. " + RDLocale.RNRN +
				"Готовое заявление можно отправить на печать или сохранить с помощью меню приложения",
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Next));

			await RDInterface.ShowMessage ("Обращаем Ваше внимание, что приложение не является официальной " +
				"разработкой Федеральной налоговой службы. Оно лишь реализует (упрощает) автоматизированное " +
				"формирование бланков заявлений и обеспечивает их соответствие (на момент публикации версии " +
				"приложения) шаблонам, зафиксированным в Приказе от 08.09.2021 N ЕД-7-20/799@. Приложение " +
				"реализовано по собственной инициативе инженеров ЦТО для ускорения работы в случаях, когда " +
				"регистрация ККТ в электронном виде вызывает затруднения",
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Next));

			await RDInterface.ShowMessage ("При этом дальнейшие операции со сформированным заявлением, " +
				"предусмотренные законодательством, выполняются пользователем самостоятельно и являются " +
				"исключительно его зоной ответственности. Разработчик никаким образом не связан с регистрирующим " +
				"органом и не отвечает за приём, передачу и обработку готовых заявлений, не передаёт их третьим " +
				"лицам и не агрегирует их. Все введённые данные остаются на устройстве пользователя и в " +
				"сформированном бланке заявления",
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
			}

		/// <summary>
		/// Обработчик события перехода в ждущий режим
		/// </summary>
		protected override void OnSleep ()
			{
			// Дамп настроек
			FlushFields ();
			KAPRSupport.SavedFields = KAPRSupport.BuildFile ();

			// Прочие настройки
			KAPRSupport.RegionIndex = (uint)addressRegionCode;
			}

		/// <summary>
		/// Запуск интерфейса
		/// </summary>
		protected override void OnStart ()
			{
			Current_MainDisplayInfoChanged (null, null);

			LoadSavedSettings ();

			base.OnStart ();
			}

		/// <summary>
		/// Возврат в интерфейс при сворачивании
		/// </summary>
		protected override void OnResume ()
			{
			RDInterface.MasterPage.PopToRootAsync (true);

			Current_MainDisplayInfoChanged (null, null);

			base.OnResume ();
			}

		// Метод восстанавливает настройки, сохранённые при выходе из приложения,
		// или сбрасывает их на начальные значения
		private void LoadSavedSettings ()
			{
			// Загрузка сохранённой версии полей
			if (KAPRSupport.ParseFile (KAPRSupport.SavedFields))
				{
				LoadFields ();
				return;
				}

			// Сброс настроек
			fnChangeFlag.IsToggled = false;
			userNameField.Text = "";
			innField.Text = "";
			ogrnField.Text = "";
			kppField.Text = "";
			presenterTypeField.Text = "";
			presenterTypeFlag.IsToggled = false;
			userNameChangeFlag.IsToggled = false;
			kktStolenFlag.IsToggled = false;
			kktMissingFlag.IsToggled = false;
			fnBrokenFlag.IsToggled = false;

			kktSerialField.Text = "";
			fnSerialField.Text = "";

			kktModel = 0;
			KKTModelButton_Clicked (null, null);

			fnModel = 0;
			FNModelButton_Clicked (null, null);

			ofdName = 0;
			OFDNameButton_Clicked (null, null);

			ofdVariant = OFDVariants.ContinueWithOFD;	// Протягивается BlankType
			kktRNMField.Text = "";

			addressRegionCode = (int)KAPRSupport.RegionIndex;
			AddressRegionCodeButton_Clicked (null, null);

			addressAreaField.Text = "";
			addressCityField.Text = "";
			addressTownField.Text = "";
			addressIndexField.Text = "";
			addressStreetField.Text = "";
			addressHouseField.Text = "";
			addressBuildingField.Text = "";
			addressAppartmentField.Text = "";
			placeField.Text = "";
			addressPlaceChangeFlag.IsToggled = false;

			lotteryFlag.IsToggled = false;
			gamblingFlag.IsToggled = false;
			gamblingExchangeFlag.IsToggled = false;
			bsoFlag.IsToggled = false;
			bankAgentFlag.IsToggled = false;
			agentFlag.IsToggled = false;
			deliveryFlag.IsToggled = false;
			exciseFlag.IsToggled = false;
			markFlag.IsToggled = false;
			internetFlag.IsToggled = false;
			otherChangeFlag.IsToggled = false;
			automatNumberField.Text = "";
			automatAddressIsSameFlag.IsToggled = false;
			automatChangeFlag.IsToggled = false;
			automatFlag.IsToggled = false;

			fnCloseFDField.Text = "";
			fnCloseDate = LibrarySupport.MinimumDatePickerValue;
			FNDateButton_Clicked (fnCloseDateButton, null);
			fnCloseFPDField.Text = "";

			fnOpenFDField.Text = "";
			fnOpenDate = LibrarySupport.MinimumDatePickerValue;
			FNDateButton_Clicked (fnOpenDateButton, null);
			fnOpenFPDField.Text = "";

			// В последнюю очередь, поскольку событие запускает внутренние проверки
			blankType = BlankTypes.RegistrationChange;
			BlankTypeButton_Clicked (null, null);
			}

		/// <summary>
		/// Возврат в интерфейс из статичного оповещения (использует перенаправление в MasterPage)
		/// </summary>
		public void ResumeApp ()
			{
			OnResume ();
			}

		// Изменение ориентации экрана
		private async void Current_MainDisplayInfoChanged (object sender, DisplayInfoChangedEventArgs e)
			{
			await Task.Delay (500);

			mainField.HeightRequest = mainField.MaximumHeightRequest = templatePage.Height -
				2.5 * createBlankButton.Height + KAPRSupport.AdditionalHeight;
			}

		// Этот вызов необходим для корректной разметки страницы журнала, когда первой отображается страница настроек
		private async void Current_LogPagePopped (object sender, NavigationEventArgs e)
			{
			Current_MainDisplayInfoChanged (null, null);
			}

		#endregion

		#region Управление полями заявления

		// Вспомогательные свойства
		private bool IsRegistrationChange
			{
			get
				{
				return (blankType == BlankTypes.RegistrationChange);
				}
			}

		private bool IsUnregistration
			{
			get
				{
				return (blankType == BlankTypes.Unregistration);
				}
			}

		// Выбор типа заявления
		private async void BlankTypeButton_Clicked (object sender, EventArgs e)
			{
			// Выбор варианта
			if (blankTypeVariants.Count < 1)
				blankTypeVariants.AddRange (KAPRSupport.BlankNames);

			int res;
			if (sender == null)
				{
				res = (int)blankType;
				}
			else
				{
				res = await RDInterface.ShowList ("Выберите тип заявления",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
					blankTypeVariants);
				if (res < 0)
					return;

				blankType = (BlankTypes)res;
				}

			blankTypeButton.Text = blankTypeVariants[res];

			// Применение настроек
			kktStolenLabel.IsVisible = kktStolenFlag.IsVisible =
				kktMissingLabel.IsVisible = kktMissingFlag.IsVisible = IsUnregistration;
			if (!IsUnregistration)
				kktStolenFlag.IsToggled = kktMissingFlag.IsToggled = false;

			fnBrokenLabel.IsVisible = fnBrokenFlag.IsVisible = IsRegistrationChange || IsUnregistration;
			if (!IsRegistrationChange && !IsUnregistration)
				fnBrokenFlag.IsToggled = false;

			userNameChangeLabel.IsVisible = userNameChangeFlag.IsVisible =
				fnChangeLabel.IsVisible = fnChangeFlag.IsVisible =
				kktRNMLabel.IsVisible = kktRNMField.IsVisible =
				addressPlaceChangeLabel.IsVisible = addressPlaceChangeFlag.IsVisible =
				automatChangeLabel.IsVisible = automatChangeFlag.IsVisible =
				otherChangeLabel.IsVisible = otherChangeFlag.IsVisible = IsRegistrationChange;
			if (!IsRegistrationChange)
				userNameChangeFlag.IsToggled = fnChangeFlag.IsToggled =
				addressPlaceChangeFlag.IsToggled = automatChangeFlag.IsToggled =
				otherChangeFlag.IsToggled = false;

			fnModelLabel.IsVisible = fnModelButton.IsVisible =
				fnSerialLabel.IsVisible = fnSerialField.IsVisible = !IsUnregistration;
			if (IsUnregistration)
				{
				fnModel = 0;
				FNModelButton_Clicked (null, null);
				}

			int ofdIdx = (int)ofdVariant;
			int ofdLimit;
			if (IsRegistrationChange || IsUnregistration)
				ofdLimit = KAPRSupport.RegistrationChangeOFDModes.Length;
			else
				ofdLimit = KAPRSupport.RegistrationOFDModes.Length;

			if (IsUnregistration || (ofdIdx >= ofdLimit))
				ofdVariant = (int)OFDVariants.ContinueWithOFD;

			ofdVariantLabel.IsVisible = ofdVariantButton.IsVisible = !IsUnregistration;
			OFDVariantButton_Clicked (null, null);

			addressRegionCodeLabel.IsVisible = addressRegionCodeButton.IsVisible =
				addressAreaLabel.IsVisible = addressAreaField.IsVisible =
				addressIndexLabel.IsVisible = addressIndexField.IsVisible =
				addressCityLabel.IsVisible = addressCityField.IsVisible =
				addressTownLabel.IsVisible = addressTownField.IsVisible =
				addressStreetLabel.IsVisible = addressStreetField.IsVisible =
				addressHouseLabel.IsVisible = addressHouseField.IsVisible =
				addressBuildingLabel.IsVisible = addressBuildingField.IsVisible =
				addressAppartmentLabel.IsVisible = addressAppartmentField.IsVisible =
				placeLabel.IsVisible = placeField.IsVisible = findPostIndexButton.IsVisible = !IsUnregistration;

			lotteryLabel.IsVisible = lotteryFlag.IsVisible =
				gamblingLabel.IsVisible = gamblingFlag.IsVisible =
				gamblingExchangeLabel.IsVisible = gamblingExchangeFlag.IsVisible =
				bankAgentLabel.IsVisible = bankAgentFlag.IsVisible =
				agentLabel.IsVisible = agentFlag.IsVisible =
				deliveryLabel.IsVisible = deliveryFlag.IsVisible =
				markLabel.IsVisible = markFlag.IsVisible =
				exciseLabel.IsVisible = exciseFlag.IsVisible =
				internetLabel.IsVisible = internetFlag.IsVisible =
				bsoLabel.IsVisible = bsoFlag.IsVisible =
				automatLabel.IsVisible = automatFlag.IsVisible = !IsUnregistration;
			if (IsUnregistration)
				lotteryFlag.IsToggled = gamblingFlag.IsToggled = gamblingExchangeFlag.IsToggled =
				bankAgentFlag.IsToggled = agentFlag.IsToggled = deliveryFlag.IsToggled =
				markFlag.IsToggled = exciseFlag.IsToggled = internetFlag.IsToggled =
				bsoFlag.IsToggled = automatFlag.IsToggled = false;

			UpdateFNOpen ();
			UpdateFNClose ();
			}

		// Изменение флагов, влияющих на ввод реквизитов закрытия архива
		private void FNChangeFlag_CheckedChanged (object sender, ToggledEventArgs e)
			{
			UpdateFNOpen ();
			UpdateFNClose ();
			}

		private void FNBrokenFlag_CheckedChanged (object sender, ToggledEventArgs e)
			{
			UpdateFNClose ();
			}

		private void UpdateFNClose ()
			{
			bool state = (fnChangeFlag.IsToggled || IsUnregistration) && !fnBrokenFlag.IsToggled;

			fnCloseFDLabel1.IsVisible = fnCloseFDLabel2.IsVisible = fnCloseFDField.IsVisible =
				fnCloseDateLabel.IsVisible = fnCloseDateButton.IsVisible =
				fnCloseFPDLabel.IsVisible = fnCloseFPDField.IsVisible = state;
			if (!state)
				{
				fnCloseDate = LibrarySupport.MinimumDatePickerValue;
				FNDateButton_Clicked (fnCloseDateButton, null);
				fnCloseFDField.Text = fnCloseFPDField.Text = "";
				}
			}

		private void UpdateFNOpen ()
			{
			fnOpenFDLabel1.IsVisible = fnOpenFDLabel2.IsVisible = fnOpenFDField.IsVisible =
				fnOpenFDField.IsEnabled =
				fnOpenDateLabel.IsVisible = fnOpenDateButton.IsVisible =
				fnOpenFPDLabel.IsVisible = fnOpenFPDField.IsVisible = IsRegistrationChange;
			if (!IsRegistrationChange)
				{
				fnOpenDate = LibrarySupport.MinimumDatePickerValue;
				FNDateButton_Clicked (fnOpenDateButton, null);
				fnOpenFDField.Text = fnOpenFPDField.Text = "";
				}
			else if (fnChangeFlag.IsToggled)
				{
				fnOpenFDField.IsEnabled = false;
				fnOpenFDField.Text = "1";
				}
			}

		// Поиск пользователя
		private async void FindUserButton_Click (object sender, EventArgs e)
			{
			// Поиск в ЕГРЮЛ
			RDInterface.ShowBalloon ("ИНН пользователя скопирован в буфер", true);
			RDGenerics.SendToClipboard (innField.Text, false);
			await RDGenerics.RunURL (KAPRSupport.UserSearchRequest, true);
			}

		// Выбор модели ККТ
		private async void KKTModelButton_Clicked (object sender, EventArgs e)
			{
			// Выбор варианта
			if (kktModelVariants.Count < 1)
				{
				kktModelVariants.Add (KAPRSupport.FillableFieldAlias);
				kktModelVariants.AddRange (kb.KKTNumbers.EnumerateAvailableModels ());
				}

			int res;
			if (sender == null)
				{
				res = kktModel;
				}
			else
				{
				res = await RDInterface.ShowList ("Выберите модель ККТ",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
					kktModelVariants);
				if (res < 0)
					return;

				kktModel = res;
				}

			kktModelButton.Text = kktModelVariants[res];
			}

		// Ввод ЗН ККТ
		private void KKTSerialField_TextChanged (object sender, EventArgs e)
			{
			string model = kb.KKTNumbers.GetKKTModel (kktSerialField.Text);
			model = model.Replace (" (неточно)", "");

			int idx = kktModelVariants.IndexOf (model);
			if (idx >= 0)
				{
				kktModel = idx;
				KKTModelButton_Clicked (null, null);
				}
			}

		// Выбор модели ФН
		private async void FNModelButton_Clicked (object sender, EventArgs e)
			{
			// Выбор варианта
			if (fnModelVariants.Count < 1)
				{
				fnModelVariants.Add (KAPRSupport.FillableFieldAlias);
				fnModelVariants.AddRange (kb.FNNumbers.EnumerateAvailableModels ());
				}

			int res;
			if (sender == null)
				{
				res = fnModel;
				}
			else
				{
				res = await RDInterface.ShowList ("Выберите модель ФН",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
					fnModelVariants);
				if (res < 0)
					return;

				fnModel = res;
				}

			fnModelButton.Text = fnModelVariants[res];
			}

		// Ввод ЗН ФН
		private void FNSerialField_TextChanged (object sender, EventArgs e)
			{
			string model = kb.FNNumbers.GetFNName (fnSerialField.Text);
			int idx = model.LastIndexOf (',');
			if (idx < 0)
				return;

			model = model.Substring (0, idx);
			idx = fnModelVariants.IndexOf (model);
			if (idx >= 0)
				{
				fnModel = idx;
				FNModelButton_Clicked (null, null);
				}
			}

		// Выбор варианта работы с ОФД
		private async void OFDVariantButton_Clicked (object sender, EventArgs e)
			{
			// Выбор варианта
			List<string> ofdVariants = [];
			if (IsRegistrationChange || IsUnregistration)
				ofdVariants.AddRange (KAPRSupport.RegistrationChangeOFDModes);
			else
				ofdVariants.AddRange (KAPRSupport.RegistrationOFDModes);

			int res;
			if (sender == null)
				{
				res = (int)ofdVariant;
				}
			else
				{
				res = await RDInterface.ShowList ("Выберите режим работы с ОФД",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
					ofdVariants);
				if (res < 0)
					return;

				ofdVariant = (OFDVariants)res;
				}

			ofdVariantButton.Text = ofdVariants[res];

			// Переключение полей
			bool state = !IsUnregistration &&
				((ofdVariant == OFDVariants.ContinueWithOFD) ||
				(ofdVariant == OFDVariants.AddOFD) || (ofdVariant == OFDVariants.ChangeOFD));
			ofdNameLabel.IsVisible = ofdNameButton.IsVisible = state;
			if (!state)
				{
				ofdName = 0;
				OFDNameButton_Clicked (null, null);
				}
			}

		// Выбор названия ОФД
		private async void OFDNameButton_Clicked (object sender, EventArgs e)
			{
			// Выбор варианта
			if (ofdNameVariants.Count < 1)
				{
				ofdNameVariants.Add (KAPRSupport.FillableFieldAlias);
				ofdNameVariants.AddRange (kb.Ofd.GetOFDNames (true));
				}

			int res;
			if (sender == null)
				{
				res = ofdName;
				}
			else
				{
				res = await RDInterface.ShowList ("Выберите ОФД",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
					ofdNameVariants);
				if (res < 0)
					return;

				ofdName = res;
				}

			ofdNameButton.Text = ofdNameVariants[res];
			}

		// Выбор региона РФ
		private async void AddressRegionCodeButton_Clicked (object sender, EventArgs e)
			{
			// Выбор варианта
			if (addressRegionCodeVariants.Count < 1)
				{
				addressRegionCodeVariants.Add (KAPRSupport.FillableFieldAlias);
				addressRegionCodeVariants.AddRange (kb.KKTNumbers.EnumerateAvailableRegions ());
				}

			int res;
			if (sender == null)
				{
				res = addressRegionCode;
				}
			else
				{
				res = await RDInterface.ShowList ("Выберите регион",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
					addressRegionCodeVariants);
				if (res < 0)
					return;

				addressRegionCode = res;
				}

			addressRegionCodeButton.Text = addressRegionCodeVariants[res];
			}

		// Поиск почтового индекса
		private async void FindPostIndex_Click (object sender, EventArgs e)
			{
			bool city = !string.IsNullOrWhiteSpace (addressCityField.Text);
			bool town = !string.IsNullOrWhiteSpace (addressTownField.Text);

			if (!city && !town)
				{
				await RDInterface.ShowMessage ("Для поиска почтового индекса нужно указать " +
					"город или населённый пункт", RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
				return;
				}

			// Населённый пункт – более точная локация
			if (town)
				{
				await RDGenerics.RunURL (KAPRSupport.AddressIndexSearchRequest +
					addressTownField.Text.Replace (' ', '+').ToLower (), true);
				return;
				}

			await RDGenerics.RunURL (KAPRSupport.AddressIndexSearchRequest +
				addressCityField.Text.Replace (' ', '+').ToLower (), true);
			}

		// Изменение автоматического режима
		private void AutomatFlag_CheckedChanged (object sender, EventArgs e)
			{
			automatNumberLabel.IsVisible = automatNumberField.IsVisible =
				automatAddressIsSameLabel.IsVisible = automatAddressIsSameFlag.IsVisible =
				automatFlag.IsToggled;
			if (!automatFlag.IsToggled)
				{
				automatAddressIsSameFlag.IsToggled = false;
				automatNumberField.Text = "";
				}
			}

		// Ввод даты и времени
		private async void FNDateButton_Clicked (object sender, EventArgs e)
			{
			Button b = (Button)sender;
			bool close = (b == fnCloseDateButton);

			// Запрос значения
			if (e != null)
				{
				string initialText;
				if (close)
					initialText = (fnCloseDate.Year > LibrarySupport.MinimumDatePickerValue.Year) ? b.Text : "";
				else
					initialText = (fnOpenDate.Year > LibrarySupport.MinimumDatePickerValue.Year) ? b.Text : "";

				string value = await RDInterface.ShowInput ("Введите дату формирования отчёта",
						"Введите дату и время в формате\n[ДД.ММ.ГГГГ ЧЧ:ММ]\nили\n[ДД.ММ.ГГ ЧЧ:ММ],\nразделяя компоненты " +
						"точками, запятыми, пробелами, двоеточиями, тире или слешами",
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK),
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
						16, Keyboard.Date, initialText);

				if (value == null)
					return;

				// Разбор
				string[] values = value.Split (dateSplitters, StringSplitOptions.RemoveEmptyEntries);
				if (values.Length != 5)
					{
					RDInterface.ShowBalloon ("Дата и / или время сформированы неправильно", true);
					return;
					}

				int d, mt, y, h, mn;
				try
					{
					d = int.Parse (values[0]);
					mt = int.Parse (values[1]);
					y = int.Parse (values[2]);
					h = int.Parse (values[3]);
					mn = int.Parse (values[4]);
					}
				catch
					{
					RDInterface.ShowBalloon ("Дата и / или время не являются числами", true);
					return;
					}

				// Контроль
				if (y < 100)
					y += 2000;
				if ((y < LibrarySupport.MinimumDatePickerValue.Year) || (y > RDGenerics.MaximumDatePickerValue.Year) ||
					(mt < 1) || (mt > 12) ||
					(d < 1) || (d > DateTime.DaysInMonth (y, mt)) ||
					(h > 23) || (mn > 59))
					{
					RDInterface.ShowBalloon ("Одно из чисел выходит за допустимые границы", true);
					return;
					}

				if (close)
					fnCloseDate = new DateTime (y, mt, d, h, mn, 0);
				else
					fnOpenDate = new DateTime (y, mt, d, h, mn, 0);
				}

			// Отображение
			if (close)
				fnCloseDateButton.Text = fnCloseDate.ToString (KAPRSupport.FullDateTimeFormat);
			else
				fnOpenDateButton.Text = fnOpenDate.ToString (KAPRSupport.FullDateTimeFormat);
			}

		#endregion

		#region Формирование заявления

		// Метод переносит значения полей из списка в интерфейс
		private bool LoadFields ()
			{
			// 11 + 5 + 2 + 11 + 15 + 6 + 2 = 52
			try
				{
				fnChangeFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.FNChangeFlag);
				userNameField.Text = KAPRSupport.GetFieldAsString (KBFFields.UserName);
				innField.Text = KAPRSupport.GetFieldAsString (KBFFields.INN);
				ogrnField.Text = KAPRSupport.GetFieldAsString (KBFFields.OGRN);
				kppField.Text = KAPRSupport.GetFieldAsString (KBFFields.KPP);
				presenterTypeField.Text = KAPRSupport.GetFieldAsString (KBFFields.UserPresenter);
				presenterTypeFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.UserPresenterType);
				userNameChangeFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.NameChangeFlag);
				kktStolenFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.KKTStolenFlag);
				kktMissingFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.KKTMissingFlag);
				fnBrokenFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.FNBrokenFlag);

				kktSerialField.Text = KAPRSupport.GetFieldAsString (KBFFields.KKTSerialNumber);
				fnSerialField.Text = KAPRSupport.GetFieldAsString (KBFFields.FNSerialNumber);
				kktModelButton.Text = KAPRSupport.GetFieldAsString (KBFFields.KKTModelName);
				fnModelButton.Text = KAPRSupport.GetFieldAsString (KBFFields.FNModelName);
				ofdNameButton.Text = KAPRSupport.GetFieldAsString (KBFFields.OFDName);

				ofdVariant = (OFDVariants)KAPRSupport.GetFieldAsUint (KBFFields.OFDVariant);    // Протягивается BlankType
				kktRNMField.Text = KAPRSupport.GetFieldAsString (KBFFields.RegistrationNumber);

				addressRegionCode = (int)KAPRSupport.GetFieldAsUint (KBFFields.AddressRegionCode);
				AddressRegionCodeButton_Clicked (null, null);

				addressAreaField.Text = KAPRSupport.GetFieldAsString (KBFFields.AddressArea);
				addressCityField.Text = KAPRSupport.GetFieldAsString (KBFFields.AddressCity);
				addressTownField.Text = KAPRSupport.GetFieldAsString (KBFFields.AddressTown);
				addressIndexField.Text = KAPRSupport.GetFieldAsString (KBFFields.AddressIndex);
				addressStreetField.Text = KAPRSupport.GetFieldAsString (KBFFields.AddressStreet);
				addressHouseField.Text = KAPRSupport.GetFieldAsString (KBFFields.AddressHouseNumber);
				addressBuildingField.Text = KAPRSupport.GetFieldAsString (KBFFields.AddressBuildingNumber);
				addressAppartmentField.Text = KAPRSupport.GetFieldAsString (KBFFields.AddressAppartmentNumber);
				placeField.Text = KAPRSupport.GetFieldAsString (KBFFields.Place);
				addressPlaceChangeFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.AddressPlaceChangeFlag);

				lotteryFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.LotteryFlag);
				gamblingFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.GamblingFlag);
				gamblingExchangeFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.GamblingExchangeFlag);
				bsoFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.BSOFlag);
				bankAgentFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.BankPaymentAgentFlag);
				agentFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.PaymentAgentFlag);
				deliveryFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.DeliveryFlag);
				exciseFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.ExciseFlag);
				markFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.MarkFlag);
				internetFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.InternetFlag);
				otherChangeFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.OtherChangeFlag);
				automatFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.AutomatFlag);     // Должен предшествовать остальным
				automatNumberField.Text = KAPRSupport.GetFieldAsString (KBFFields.AutomatNumber);
				automatAddressIsSameFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.AutomatAddressIsSameFlag);
				automatChangeFlag.IsToggled = KAPRSupport.GetFieldAsBool (KBFFields.AutomatChangeFlag);

				fnCloseFDField.Text = KAPRSupport.GetFieldAsString (KBFFields.FNCloseDocumentNumber);
				fnCloseDate = KAPRSupport.GetFieldAsDateTime (KBFFields.FNCloseDate);
				FNDateButton_Clicked (fnCloseDateButton, null);
				fnCloseFPDField.Text = KAPRSupport.GetFieldAsString (KBFFields.FNCloseDocumentSign);

				fnOpenFDField.Text = KAPRSupport.GetFieldAsString (KBFFields.FNOpenDocumentNumber);
				fnOpenDate = KAPRSupport.GetFieldAsDateTime (KBFFields.FNOpenDate);
				FNDateButton_Clicked (fnOpenDateButton, null);
				fnOpenFPDField.Text = KAPRSupport.GetFieldAsString (KBFFields.FNOpenDocumentSign);

				// В последнюю очередь, поскольку событие запускает внутренние проверки
				blankType = (BlankTypes)KAPRSupport.GetFieldAsUint (KBFFields.BlankType);
				// Версия файла здесь игнорируется
				BlankTypeButton_Clicked (null, null);
				}
			catch
				{
				return false;
				}

			return true;
			}

		// Метод переносит значения полей из интерфейса в список
		private void FlushFields ()
			{
			// 13 + 5 + 2 + 11 + 15 + 6 = 52
			KAPRSupport.SetField (KBFFields.FileVersion, (uint)KBFVersions.Actual);
			KAPRSupport.SetField (KBFFields.BlankType, (uint)blankType);
			KAPRSupport.SetField (KBFFields.FNChangeFlag, fnChangeFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.UserName, userNameField.Text);
			KAPRSupport.SetField (KBFFields.INN, innField.Text);
			KAPRSupport.SetField (KBFFields.OGRN, ogrnField.Text);
			KAPRSupport.SetField (KBFFields.KPP, kppField.Text);
			KAPRSupport.SetField (KBFFields.UserPresenter, presenterTypeField.Text);
			KAPRSupport.SetField (KBFFields.UserPresenterType, presenterTypeFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.NameChangeFlag, userNameChangeFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.KKTStolenFlag, kktStolenFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.KKTMissingFlag, kktMissingFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.FNBrokenFlag, fnBrokenFlag.IsToggled);

			KAPRSupport.SetField (KBFFields.KKTSerialNumber, kktSerialField.Text);
			KAPRSupport.SetField (KBFFields.KKTModelName, kktModelButton.Text);
			KAPRSupport.SetField (KBFFields.FNSerialNumber, fnSerialField.Text);
			KAPRSupport.SetField (KBFFields.FNModelName, fnModelButton.Text);
			KAPRSupport.SetField (KBFFields.OFDName, ofdNameButton.Text);

			KAPRSupport.SetField (KBFFields.OFDVariant, (uint)ofdVariant);
			KAPRSupport.SetField (KBFFields.RegistrationNumber, kktRNMField.Text);

			KAPRSupport.SetField (KBFFields.AddressRegionCode, (uint)addressRegionCode);
			KAPRSupport.SetField (KBFFields.AddressArea, addressAreaField.Text);
			KAPRSupport.SetField (KBFFields.AddressCity, addressCityField.Text);
			KAPRSupport.SetField (KBFFields.AddressTown, addressTownField.Text);
			KAPRSupport.SetField (KBFFields.AddressIndex, addressIndexField.Text);
			KAPRSupport.SetField (KBFFields.AddressStreet, addressStreetField.Text);
			KAPRSupport.SetField (KBFFields.AddressHouseNumber, addressHouseField.Text);
			KAPRSupport.SetField (KBFFields.AddressBuildingNumber, addressBuildingField.Text);
			KAPRSupport.SetField (KBFFields.AddressAppartmentNumber, addressAppartmentField.Text);
			KAPRSupport.SetField (KBFFields.Place, placeField.Text);
			KAPRSupport.SetField (KBFFields.AddressPlaceChangeFlag, addressPlaceChangeFlag.IsToggled);

			KAPRSupport.SetField (KBFFields.LotteryFlag, lotteryFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.GamblingFlag, gamblingFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.GamblingExchangeFlag, gamblingExchangeFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.BSOFlag, bsoFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.BankPaymentAgentFlag, bankAgentFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.PaymentAgentFlag, agentFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.DeliveryFlag, deliveryFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.ExciseFlag, exciseFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.MarkFlag, markFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.InternetFlag, internetFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.OtherChangeFlag, otherChangeFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.AutomatNumber, automatNumberField.Text);
			KAPRSupport.SetField (KBFFields.AutomatAddressIsSameFlag, automatAddressIsSameFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.AutomatChangeFlag, automatChangeFlag.IsToggled);
			KAPRSupport.SetField (KBFFields.AutomatFlag, automatFlag.IsToggled);

			KAPRSupport.SetField (KBFFields.FNCloseDocumentNumber, fnCloseFDField.Text);
			KAPRSupport.SetField (KBFFields.FNCloseDate, fnCloseDate);
			KAPRSupport.SetField (KBFFields.FNCloseDocumentSign, fnCloseFPDField.Text);
			KAPRSupport.SetField (KBFFields.FNOpenDocumentNumber, fnOpenFDField.Text);
			KAPRSupport.SetField (KBFFields.FNOpenDate, fnOpenDate);
			KAPRSupport.SetField (KBFFields.FNOpenDocumentSign, fnOpenFPDField.Text);
			}

		// Формирование заявления
		private async void CreateBlank_Click (object sender, EventArgs e)
			{
			// Сохранение полей
			FlushFields ();
			KAPRSupport.SavedFields = KAPRSupport.BuildFile ();

			// Контроль значений
			switch (KAPRSupport.CheckFields (IsRegistrationChange, addressIndexField.IsEnabled,
				fnOpenDateButton.IsEnabled && fnCloseDateButton.IsEnabled))
				{
				case -1:
					innField.Focus ();
					await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));

					RDGenerics.HideKeyboard (innField);
					return;

				case -2:
					ogrnField.Focus ();
					await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));

					RDGenerics.HideKeyboard (ogrnField);
					return;

				case -3:
					kppField.Focus ();
					await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));

					RDGenerics.HideKeyboard (kppField);
					return;

				case -4:
					fnSerialField.Focus ();
					await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));

					RDGenerics.HideKeyboard (fnSerialField);
					return;

				case -5:
					kktRNMField.Focus ();
					await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));

					RDGenerics.HideKeyboard (kktRNMField);
					return;

				case -6:
					addressIndexField.Focus ();
					await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));

					RDGenerics.HideKeyboard (addressIndexField);
					return;

				case -7:
					fnCloseFDField.Focus ();
					await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));

					RDGenerics.HideKeyboard (fnCloseFDField);
					return;

				case -8:
					fnCloseFPDField.Focus ();
					await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));

					RDGenerics.HideKeyboard (fnCloseFPDField);
					return;

				case -9:
					await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
					return;

				case -10:
					kktRNMField.Focus ();
					await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));

					RDGenerics.HideKeyboard (kktRNMField);
					return;

				case -11:
					if (!await RDInterface.ShowMessage (KAPRSupport.CheckFieldsError,
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_Next),
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel)))
						{
						return;
						}

					break;
				}

			// Формирование заявления
			string template = KAPRSupport.BuildTemplate (kb);

			// Запуск на печать
			KKTSupport.PrintBlank (KAPRSupport.GetRecommendedFileName (userNameField.Text, blankType),
				template, KAPRSupport.GetPagesCount (blankType));
			}

		#endregion

		#region Настройки и О приложении

		// Вызов меню программы
		private async void MenuButton_Click (object sender, EventArgs e)
			{
			// Запрос варианта
			if (menuVariants.Count < 1)
				{
				menuVariants.Add ("⬆️\t Загрузить из файла");
				menuVariants.Add ("⬇️\t Сохранить в файл");
				menuVariants.Add ("❌\t Сбросить все поля");
				menuVariants.Add ("⚙️\t Настройки");
				menuVariants.Add ("ℹ️\t Справка и поддержка");
				}

			int res = await RDInterface.ShowList ("Меню", RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
				menuVariants);
			if (res < 0)
				return;

			// Разбор
			switch (res)
				{
				// Загрузка файла
				case 0:
					string inFile = await RDGenerics.LoadFromFile (RDEncodings.UTF8);
					if (string.IsNullOrWhiteSpace (inFile))
						return;

					if (!KAPRSupport.ParseFile (inFile))
						{
						await RDInterface.ShowMessage ("Не удалось загрузить указанный файл." + RDLocale.RNRN +
							"Возможно, выбранный файл заявления имел версию, не поддерживаемую Android-приложением. " +
							"Попробуйте пересохранить его с помощью Windows-клиента и повторите попытку",
							RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
						return;
						}

					if (LoadFields ())
						RDInterface.ShowBalloon ("Файл успешно загружен", false);
					else
						await RDInterface.ShowMessage ("Файл заявления повреждён и не может быть загружен полностью. " +
							"Проверьте поля заявления перед формированием", RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
					break;

				// Сохранение файла
				case 1:
					FlushFields ();
					string outFile = KAPRSupport.BuildFile ();

					await RDGenerics.SaveToFile (KAPRSupport.GetRecommendedFileName (userNameField.Text,
						blankType) + KAPRSupport.BlankFileExtension, outFile, RDEncodings.UTF8);
					break;

				// Сброс полей
				case 2:
					if (!await RDInterface.ShowMessage ("Сбросить значения всех полей заявления?",
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_Yes),
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel)))
						return;

					KAPRSupport.SavedFields = "";
					LoadSavedSettings ();

					RDInterface.ShowBalloon ("Значения полей сброшены", false);
					break;

				// Настройки
				case 3:
					RDInterface.SetCurrentPage (settingsPage, settingsMasterBackColor);
					break;

				// О приложении
				case 4:
					RDInterface.SetCurrentPage (aboutPage, aboutMasterBackColor);
					break;
				}
			}

		// Вызов справочных материалов
		private async void ReferenceButton_Click (object sender, EventArgs e)
			{
			await RDInterface.CallHelpMaterials (RDHelpMaterials.ReferenceMaterials);
			}

		private async void HelpButton_Click (object sender, EventArgs e)
			{
			await RDInterface.CallHelpMaterials (RDHelpMaterials.HelpAndSupport);
			}

		// Изменение размера шрифта интерфейса
		private void FontSizeButton_Clicked (object sender, EventArgs e)
			{
			if (sender != null)
				{
				Button b = (Button)sender;
				if (RDInterface.IsNameDefault (b.Text, RDDefaultButtons.Increase))
					RDInterface.MasterFontSize += 0.5;
				else if (RDInterface.IsNameDefault (b.Text, RDDefaultButtons.Decrease))
					RDInterface.MasterFontSize -= 0.5;
				}

			fontSizeField.Text = RDInterface.MasterFontSize.ToString ("F1");
			fontSizeField.FontSize = RDInterface.MasterFontSize;
			}

		// Переключение настроек
		private void DontAddStrikeouts_Toggled (object sender, EventArgs e)
			{
			KAPRSupport.DontAddStrikeouts = dontAddStrikeoutsFlag.IsToggled;
			}

		private void AddSignDate_Toggled (object sender, EventArgs e)
			{
			KAPRSupport.AddSignDate = addSignDateFlag.IsToggled;
			}

		// Переключение ограничителя высоты журнала
		private async void SwitchHeightFlag_Toggled (object sender, EventArgs e)
			{
			if (KAPRSupport.AdditionalHeight != 0)
				KAPRSupport.AdditionalHeight = 0;
			else
				KAPRSupport.AdditionalHeight = (uint)createBlankButton.Height;

			Current_MainDisplayInfoChanged (null, null);
			}

		#endregion
		}
	}
