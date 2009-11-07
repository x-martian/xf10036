#pragma once

#define IMAGEWIDTH 512
#define IMAGEHEIGHT 512
#define IMAGELENGTH 1024

namespace Render2DCLR {

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace System::Drawing::Imaging;
	using namespace System::Text;
	using namespace System::Runtime::InteropServices;

    [DllImport("kernel32.dll")]
    int QueryPerformanceCounter(Int64% count);
    [DllImport("kernel32.dll")]
    int QueryPerformanceFrequency(Int64% frequency);

	/// <summary>
	/// Summary for Form1
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class Form1 : public System::Windows::Forms::Form
	{
	public:
		Form1(void) : currentImage(0), go(false)
		{
			InitializeComponent();

			imageArray = gcnew array<short>(IMAGEWIDTH*IMAGEHEIGHT*IMAGELENGTH);
			System::Random^ rand = gcnew System::Random(1);
			int p = 0;
			for (unsigned k=0; k<IMAGELENGTH; ++k)
				for (unsigned j=0; j<IMAGEHEIGHT; ++j)
					for (unsigned i=0; i<IMAGEWIDTH; ++i) {
						//short v = rand->Next(2048);
						short v = i+j+k/2;
						if (v>2047) v=2047;
						imageArray[p] = v;
						++p;
					}

			lut = gcnew array<Byte>(2048);
			for (short b=0; b<2048; ++b)
			{
				lut[b] = b*256/2048;
			}

			dispArray = gcnew array<System::Byte>(300*300*IMAGELENGTH);
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~Form1()
		{
			if (components)
			{
				delete components;
			}
		}

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container ^components;
		array<short>^ imageArray;
		array<System::Byte>^ dispArray;
		array<Byte>^ lut;
		int currentImage;
		Boolean go;
		Bitmap^ bitmap;

	private: System::Windows::Forms::MenuStrip^  menuStrip1;
	private: System::Windows::Forms::ToolStripMenuItem^  fileToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  newToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  openToolStripMenuItem;
	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator;
	private: System::Windows::Forms::ToolStripMenuItem^  saveToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  saveAsToolStripMenuItem;
	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator1;
	private: System::Windows::Forms::ToolStripMenuItem^  printToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  printPreviewToolStripMenuItem;
	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator2;
	private: System::Windows::Forms::ToolStripMenuItem^  exitToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  editToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  undoToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  redoToolStripMenuItem;
	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator3;
	private: System::Windows::Forms::ToolStripMenuItem^  cutToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  copyToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  pasteToolStripMenuItem;
	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator4;
	private: System::Windows::Forms::ToolStripMenuItem^  selectAllToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  toolsToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  customizeToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  optionsToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  helpToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  contentsToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  indexToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  searchToolStripMenuItem;
	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator5;
	private: System::Windows::Forms::ToolStripMenuItem^  aboutToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  actionToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  raceToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  mathToolStripMenuItem;
			 Int64 tmr;


#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(Form1::typeid));
			this->menuStrip1 = (gcnew System::Windows::Forms::MenuStrip());
			this->fileToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->newToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->openToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->saveToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->saveAsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator1 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->printToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->printPreviewToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator2 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->exitToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->editToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->undoToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->redoToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator3 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->cutToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->copyToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->pasteToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator4 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->selectAllToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->actionToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->raceToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->mathToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->customizeToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->optionsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->helpToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->contentsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->indexToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->searchToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator5 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->aboutToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->menuStrip1->SuspendLayout();
			this->SuspendLayout();
			// 
			// menuStrip1
			// 
			this->menuStrip1->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(5) {this->fileToolStripMenuItem, 
				this->editToolStripMenuItem, this->actionToolStripMenuItem, this->toolsToolStripMenuItem, this->helpToolStripMenuItem});
			this->menuStrip1->Location = System::Drawing::Point(0, 0);
			this->menuStrip1->Name = L"menuStrip1";
			this->menuStrip1->Size = System::Drawing::Size(292, 24);
			this->menuStrip1->TabIndex = 0;
			this->menuStrip1->Text = L"menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this->fileToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(10) {this->newToolStripMenuItem, 
				this->openToolStripMenuItem, this->toolStripSeparator, this->saveToolStripMenuItem, this->saveAsToolStripMenuItem, this->toolStripSeparator1, 
				this->printToolStripMenuItem, this->printPreviewToolStripMenuItem, this->toolStripSeparator2, this->exitToolStripMenuItem});
			this->fileToolStripMenuItem->Name = L"fileToolStripMenuItem";
			this->fileToolStripMenuItem->Size = System::Drawing::Size(35, 20);
			this->fileToolStripMenuItem->Text = L"&File";
			// 
			// newToolStripMenuItem
			// 
			this->newToolStripMenuItem->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"newToolStripMenuItem.Image")));
			this->newToolStripMenuItem->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->newToolStripMenuItem->Name = L"newToolStripMenuItem";
			this->newToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::N));
			this->newToolStripMenuItem->Size = System::Drawing::Size(140, 22);
			this->newToolStripMenuItem->Text = L"&New";
			// 
			// openToolStripMenuItem
			// 
			this->openToolStripMenuItem->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"openToolStripMenuItem.Image")));
			this->openToolStripMenuItem->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->openToolStripMenuItem->Name = L"openToolStripMenuItem";
			this->openToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::O));
			this->openToolStripMenuItem->Size = System::Drawing::Size(140, 22);
			this->openToolStripMenuItem->Text = L"&Open";
			// 
			// toolStripSeparator
			// 
			this->toolStripSeparator->Name = L"toolStripSeparator";
			this->toolStripSeparator->Size = System::Drawing::Size(137, 6);
			// 
			// saveToolStripMenuItem
			// 
			this->saveToolStripMenuItem->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"saveToolStripMenuItem.Image")));
			this->saveToolStripMenuItem->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->saveToolStripMenuItem->Name = L"saveToolStripMenuItem";
			this->saveToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::S));
			this->saveToolStripMenuItem->Size = System::Drawing::Size(140, 22);
			this->saveToolStripMenuItem->Text = L"&Save";
			// 
			// saveAsToolStripMenuItem
			// 
			this->saveAsToolStripMenuItem->Name = L"saveAsToolStripMenuItem";
			this->saveAsToolStripMenuItem->Size = System::Drawing::Size(140, 22);
			this->saveAsToolStripMenuItem->Text = L"Save &As";
			// 
			// toolStripSeparator1
			// 
			this->toolStripSeparator1->Name = L"toolStripSeparator1";
			this->toolStripSeparator1->Size = System::Drawing::Size(137, 6);
			// 
			// printToolStripMenuItem
			// 
			this->printToolStripMenuItem->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"printToolStripMenuItem.Image")));
			this->printToolStripMenuItem->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->printToolStripMenuItem->Name = L"printToolStripMenuItem";
			this->printToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::P));
			this->printToolStripMenuItem->Size = System::Drawing::Size(140, 22);
			this->printToolStripMenuItem->Text = L"&Print";
			// 
			// printPreviewToolStripMenuItem
			// 
			this->printPreviewToolStripMenuItem->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"printPreviewToolStripMenuItem.Image")));
			this->printPreviewToolStripMenuItem->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->printPreviewToolStripMenuItem->Name = L"printPreviewToolStripMenuItem";
			this->printPreviewToolStripMenuItem->Size = System::Drawing::Size(140, 22);
			this->printPreviewToolStripMenuItem->Text = L"Print Pre&view";
			// 
			// toolStripSeparator2
			// 
			this->toolStripSeparator2->Name = L"toolStripSeparator2";
			this->toolStripSeparator2->Size = System::Drawing::Size(137, 6);
			// 
			// exitToolStripMenuItem
			// 
			this->exitToolStripMenuItem->Name = L"exitToolStripMenuItem";
			this->exitToolStripMenuItem->Size = System::Drawing::Size(140, 22);
			this->exitToolStripMenuItem->Text = L"E&xit";
			// 
			// editToolStripMenuItem
			// 
			this->editToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(8) {this->undoToolStripMenuItem, 
				this->redoToolStripMenuItem, this->toolStripSeparator3, this->cutToolStripMenuItem, this->copyToolStripMenuItem, this->pasteToolStripMenuItem, 
				this->toolStripSeparator4, this->selectAllToolStripMenuItem});
			this->editToolStripMenuItem->Name = L"editToolStripMenuItem";
			this->editToolStripMenuItem->Size = System::Drawing::Size(37, 20);
			this->editToolStripMenuItem->Text = L"&Edit";
			// 
			// undoToolStripMenuItem
			// 
			this->undoToolStripMenuItem->Name = L"undoToolStripMenuItem";
			this->undoToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::Z));
			this->undoToolStripMenuItem->Size = System::Drawing::Size(139, 22);
			this->undoToolStripMenuItem->Text = L"&Undo";
			// 
			// redoToolStripMenuItem
			// 
			this->redoToolStripMenuItem->Name = L"redoToolStripMenuItem";
			this->redoToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::Y));
			this->redoToolStripMenuItem->Size = System::Drawing::Size(139, 22);
			this->redoToolStripMenuItem->Text = L"&Redo";
			// 
			// toolStripSeparator3
			// 
			this->toolStripSeparator3->Name = L"toolStripSeparator3";
			this->toolStripSeparator3->Size = System::Drawing::Size(136, 6);
			// 
			// cutToolStripMenuItem
			// 
			this->cutToolStripMenuItem->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"cutToolStripMenuItem.Image")));
			this->cutToolStripMenuItem->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->cutToolStripMenuItem->Name = L"cutToolStripMenuItem";
			this->cutToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::X));
			this->cutToolStripMenuItem->Size = System::Drawing::Size(139, 22);
			this->cutToolStripMenuItem->Text = L"Cu&t";
			// 
			// copyToolStripMenuItem
			// 
			this->copyToolStripMenuItem->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"copyToolStripMenuItem.Image")));
			this->copyToolStripMenuItem->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->copyToolStripMenuItem->Name = L"copyToolStripMenuItem";
			this->copyToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::C));
			this->copyToolStripMenuItem->Size = System::Drawing::Size(139, 22);
			this->copyToolStripMenuItem->Text = L"&Copy";
			// 
			// pasteToolStripMenuItem
			// 
			this->pasteToolStripMenuItem->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"pasteToolStripMenuItem.Image")));
			this->pasteToolStripMenuItem->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->pasteToolStripMenuItem->Name = L"pasteToolStripMenuItem";
			this->pasteToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::V));
			this->pasteToolStripMenuItem->Size = System::Drawing::Size(139, 22);
			this->pasteToolStripMenuItem->Text = L"&Paste";
			// 
			// toolStripSeparator4
			// 
			this->toolStripSeparator4->Name = L"toolStripSeparator4";
			this->toolStripSeparator4->Size = System::Drawing::Size(136, 6);
			// 
			// selectAllToolStripMenuItem
			// 
			this->selectAllToolStripMenuItem->Name = L"selectAllToolStripMenuItem";
			this->selectAllToolStripMenuItem->Size = System::Drawing::Size(139, 22);
			this->selectAllToolStripMenuItem->Text = L"Select &All";
			// 
			// actionToolStripMenuItem
			// 
			this->actionToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {this->raceToolStripMenuItem, 
				this->mathToolStripMenuItem});
			this->actionToolStripMenuItem->Name = L"actionToolStripMenuItem";
			this->actionToolStripMenuItem->Size = System::Drawing::Size(49, 20);
			this->actionToolStripMenuItem->Text = L"&Action";
			// 
			// raceToolStripMenuItem
			// 
			this->raceToolStripMenuItem->Name = L"raceToolStripMenuItem";
			this->raceToolStripMenuItem->Size = System::Drawing::Size(152, 22);
			this->raceToolStripMenuItem->Text = L"&Overall";
			this->raceToolStripMenuItem->Click += gcnew System::EventHandler(this, &Form1::raceToolStripMenuItem_Click);
			// 
			// mathToolStripMenuItem
			// 
			this->mathToolStripMenuItem->Name = L"mathToolStripMenuItem";
			this->mathToolStripMenuItem->Size = System::Drawing::Size(152, 22);
			this->mathToolStripMenuItem->Text = L"&Math";
			this->mathToolStripMenuItem->Click += gcnew System::EventHandler(this, &Form1::mathToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this->toolsToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {this->customizeToolStripMenuItem, 
				this->optionsToolStripMenuItem});
			this->toolsToolStripMenuItem->Name = L"toolsToolStripMenuItem";
			this->toolsToolStripMenuItem->Size = System::Drawing::Size(44, 20);
			this->toolsToolStripMenuItem->Text = L"&Tools";
			// 
			// customizeToolStripMenuItem
			// 
			this->customizeToolStripMenuItem->Name = L"customizeToolStripMenuItem";
			this->customizeToolStripMenuItem->Size = System::Drawing::Size(123, 22);
			this->customizeToolStripMenuItem->Text = L"&Customize";
			// 
			// optionsToolStripMenuItem
			// 
			this->optionsToolStripMenuItem->Name = L"optionsToolStripMenuItem";
			this->optionsToolStripMenuItem->Size = System::Drawing::Size(123, 22);
			this->optionsToolStripMenuItem->Text = L"&Options";
			// 
			// helpToolStripMenuItem
			// 
			this->helpToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(5) {this->contentsToolStripMenuItem, 
				this->indexToolStripMenuItem, this->searchToolStripMenuItem, this->toolStripSeparator5, this->aboutToolStripMenuItem});
			this->helpToolStripMenuItem->Name = L"helpToolStripMenuItem";
			this->helpToolStripMenuItem->Size = System::Drawing::Size(40, 20);
			this->helpToolStripMenuItem->Text = L"&Help";
			// 
			// contentsToolStripMenuItem
			// 
			this->contentsToolStripMenuItem->Name = L"contentsToolStripMenuItem";
			this->contentsToolStripMenuItem->Size = System::Drawing::Size(118, 22);
			this->contentsToolStripMenuItem->Text = L"&Contents";
			// 
			// indexToolStripMenuItem
			// 
			this->indexToolStripMenuItem->Name = L"indexToolStripMenuItem";
			this->indexToolStripMenuItem->Size = System::Drawing::Size(118, 22);
			this->indexToolStripMenuItem->Text = L"&Index";
			// 
			// searchToolStripMenuItem
			// 
			this->searchToolStripMenuItem->Name = L"searchToolStripMenuItem";
			this->searchToolStripMenuItem->Size = System::Drawing::Size(118, 22);
			this->searchToolStripMenuItem->Text = L"&Search";
			// 
			// toolStripSeparator5
			// 
			this->toolStripSeparator5->Name = L"toolStripSeparator5";
			this->toolStripSeparator5->Size = System::Drawing::Size(115, 6);
			// 
			// aboutToolStripMenuItem
			// 
			this->aboutToolStripMenuItem->Name = L"aboutToolStripMenuItem";
			this->aboutToolStripMenuItem->Size = System::Drawing::Size(118, 22);
			this->aboutToolStripMenuItem->Text = L"&About...";
			// 
			// Form1
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(292, 271);
			this->Controls->Add(this->menuStrip1);
			this->DoubleBuffered = true;
			this->MainMenuStrip = this->menuStrip1;
			this->Name = L"Form1";
			this->Text = L"Render 2D CLR";
			this->Load += gcnew System::EventHandler(this, &Form1::Form1_Load);
			this->Paint += gcnew System::Windows::Forms::PaintEventHandler(this, &Form1::Form1_Paint);
			this->SizeChanged += gcnew System::EventHandler(this, &Form1::Form1_SizeChanged);
			this->MouseMove += gcnew System::Windows::Forms::MouseEventHandler(this, &Form1::Form1_MouseMove);
			this->menuStrip1->ResumeLayout(false);
			this->menuStrip1->PerformLayout();
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion
	private: System::Void Form1_Load(System::Object^  sender, System::EventArgs^  e) {
			bitmap = gcnew Bitmap(ClientRectangle.Width, ClientRectangle.Height, CreateGraphics());
			ClientSize = System::Drawing::Size(300, 300);
			 }
	private:
		System::Void Form1_Paint(System::Object^  sender, System::Windows::Forms::PaintEventArgs^  e) {
			//CBrush brush;
			//brush.CreateSysColorBrush(0);

//			BufferedGraphicsContext^ context = BufferedGraphicsManager::Current;
//			BufferedGraphics^ graphics = context->Allocate(this->CreateGraphics(), this->DisplayRectangle);
			System::Drawing::Rectangle rect = ClientRectangle;
			pin_ptr<short> tmp = &imageArray[0];
			short* p0 = (short*)tmp + IMAGEWIDTH*IMAGEHEIGHT*currentImage;

			array<double>^ interp = gcnew array<double>(IMAGEWIDTH+1);
			// set value at each pixel, use linear interplation
			short* lo;
			short* hi;
			Byte v;
			int index = 0;

			BitmapData^ bd = bitmap->LockBits(Rectangle(0, 0, rect.Width, rect.Height), Imaging::ImageLockMode::WriteOnly, bitmap->PixelFormat);
			IntPtr ptr = bd->Scan0;
//			int bytes = bd->Stride * rect.Height;
//			array<Byte>^ rgbValues = gcnew array<Byte>(bytes);

			// Copy the RGB values into the array.
//			Marshal::Copy(ptr, rgbValues, 0, bytes);
			Byte* p = (Byte*)(void*)ptr;
			for (int j=0; j<rect.Height; ++j) {
				pin_ptr<double> tmp = &interp[0];
				double* mid = tmp;
				double x = double((2*j+1)*IMAGEHEIGHT-rect.Height)/double(2*rect.Height);
				if (x>0.0) {
					int index = x;
					x -= index;
					lo = p0+index*IMAGEWIDTH;
					hi = lo+IMAGEWIDTH;
				} else {
					x += 1.0;
					hi = p0;
					if (hi>0)
						lo = hi - IMAGEWIDTH;
					else
						lo = p0 + IMAGEWIDTH*(IMAGEHEIGHT*IMAGELENGTH-1);
				}
				double y = 1.0-x;
				for (int m=0; m<IMAGEWIDTH; ++m) {
					*mid = *lo*y+*hi*x;
					++mid; ++lo; ++hi;
				}
				*mid = interp[0];	// repeat the first point at the end
				for (int i=0; i<rect.Width; ++i) {
					x = double((2*i+1)*IMAGEWIDTH-rect.Width)/double(2*rect.Width);
					if (x>0.0)
					{
						int index = x;
						x -= index;
						double s = *(tmp+index)*(1-x)+*(tmp+index+1)*x;
						v = lut[(int)s];
						//rgbValues[offset++] = v;
						//rgbValues[offset++] = v;
						//rgbValues[offset++] = v;
						//rgbValues[offset++] = 255;

						//Marshal::WriteByte(ptr, offset++, v);
						//Marshal::WriteByte(ptr, offset++, v);
						//Marshal::WriteByte(ptr, offset++, v);
						//Marshal::WriteByte(ptr, offset++, 255);
						*p = v; ++p; *p = v; ++p; *p = v; ++p; *p = 255; ++p;
//						bitmap->SetPixel(i, j, Color::FromArgb(v, v, v));
					} else p += 4;
				}
			}
			// Copy the RGB values back to the bitmap
//			for (int k=0; k<1024; ++k)
//			{
//				*p = 0; ++p;
			//	rgbValues[offset++] = 0;
//			}
			//Marshal::Copy(rgbValues, 0, ptr, bytes);

			bitmap->UnlockBits(bd);

			e->Graphics->DrawImage(bitmap, 0, 0);
			++currentImage;
			if (go==true) {
				if (currentImage < IMAGELENGTH-1)
					this->Invalidate(rect);
				else {
					Int64 now, freq;
					QueryPerformanceCounter(now);
					QueryPerformanceFrequency(freq);
					Int64 span = now-tmr;
					Int64 fact = freq;
					while (span > 10000) {
						span = span >> 1;
						fact = fact >> 1;
					}

					double fps = double(currentImage)*fact/double(span);
					StringBuilder^ sb = gcnew StringBuilder();
					sb->AppendFormat("fps: {0}", fps);
					MessageBox::Show(sb->ToString());
					currentImage = 0;
					go = false;
					Invalidate();
				}
			} else if (currentImage == IMAGELENGTH-1)
				currentImage = 0;
		}
	private:
		System::Void raceToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) {
			go = true;
			currentImage = 0;
			QueryPerformanceCounter(tmr);
			Invalidate();
		}
private: System::Void Form1_SizeChanged(System::Object^  sender, System::EventArgs^  e) {
			 if (ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)
				 return;

			bitmap = gcnew Bitmap(ClientRectangle.Width, ClientRectangle.Height, CreateGraphics());
			Invalidate();
		 }
private: System::Void Form1_MouseMove(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
			 Invalidate();
		 }
		 
private:
	System::Void mathToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e)
	{
		System::Drawing::Rectangle rect = ClientRectangle;
		pin_ptr<Int16> tmp = &imageArray[0];
		currentImage = 0;
		int w=300, h=300;
        Int64 now, delta;
		QueryPerformanceCounter(tmr);
        QueryPerformanceCounter(now);
		delta = now-tmr;	// find the p/invoke penalty
		QueryPerformanceCounter(tmr);
		array<System::Double>^ interp = gcnew array<double>(IMAGEWIDTH+1);
		do {
			Int16* p0 = (Int16*)tmp + IMAGEWIDTH*IMAGEHEIGHT*currentImage;
			pin_ptr<Byte> ppv = &dispArray[300*300*currentImage];
			Byte* pv = (Byte*)ppv;
			// set value at each pixel, use linear interplation
			Int16* lo;
			Int16* hi;
			Byte v;
			int index = 0;

			for (int j=0; j<h; ++j) {
				pin_ptr<double> tmp = &interp[0];
				double* mid = tmp;
//				double x=0.001;
				double x = double((2*j+1)*IMAGEHEIGHT-h)/double(2*h);
				if (x>0.0) {
					int index = x;
					x -= index;
					lo = p0+index*IMAGEWIDTH;
					hi = lo+IMAGEWIDTH;
				} else {
					x += 1.0;
					hi = p0;
					if (hi>0)
						lo = hi - IMAGEWIDTH;
					else
						lo = p0 + IMAGEWIDTH*(IMAGEHEIGHT*IMAGELENGTH-1);
				}
				double y = 1.0-x;
				for (int m=0; m<IMAGEWIDTH; ++m) {
					*mid = *lo*y+*hi*x;
					++mid; ++lo; ++hi;
				}
				*mid = interp[0];	// repeat the first point at the end
				for (int i=0; i<w; ++i) {
					x = double((2*i+1)*IMAGEWIDTH-w)/double(2*w);
					if (x>0.0)
					{
						int index = x;
						x -= index;
						double s = *(tmp+index)*(1-x)+*(tmp+index+1)*x;
						v = lut[(int)s];
						*pv = v;
						++pv;
					}
				}
			}
		} while (++currentImage<IMAGELENGTH-1);

        Int64 fact;
        QueryPerformanceCounter(now);
        QueryPerformanceFrequency(fact);
        Int64 span = now - tmr - delta;
        while (span > 10000)
        {
            span = span >> 1;
            fact = fact >> 1;
        }

        double fps = (double)(currentImage) * fact / (double)(span);
        StringBuilder^ sb = gcnew StringBuilder();
        sb->AppendFormat("fps: {0}", fps);
        currentImage = 0;
		MessageBox::Show(sb->ToString());
	}
};
}

