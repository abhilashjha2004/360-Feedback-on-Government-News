import sys
from pptx import Presentation
from pptx.util import Inches, Pt
from pptx.enum.text import PP_ALIGN
from pptx.dml.color import RGBColor
from pptx.enum.shapes import MSO_SHAPE

def create_deck():
    prs = Presentation()
    # 16:9 widescreen
    prs.slide_width = Inches(13.333)
    prs.slide_height = Inches(7.5)
    blank_layout = prs.slide_layouts[6]

    # Professional Color Palette
    PRIMARY = RGBColor(15, 32, 67)      # Deep Navy #0F2043
    SECONDARY = RGBColor(30, 58, 138)   # Royal Blue #1E3A8A
    ACCENT = RGBColor(217, 119, 6)      # Warm Saffron / Gold #D97706
    ACCENT_GREEN = RGBColor(13, 148, 136)# Teal Green #0D9488
    BG_LIGHT = RGBColor(248, 250, 252)  # Slate 50 #F8FAFC
    CARD_BG = RGBColor(255, 255, 255)   # White
    CARD_BORDER = RGBColor(226, 232, 240)# Slate 200 #E2E8F0
    TEXT_DARK = RGBColor(15, 23, 42)    # Slate 900
    TEXT_MUTED = RGBColor(100, 116, 139)# Slate 500
    WHITE = RGBColor(255, 255, 255)

    def set_slide_bg(slide, color=BG_LIGHT):
        bg = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, 0, 0, prs.slide_width, prs.slide_height)
        bg.fill.solid()
        bg.fill.fore_color.rgb = color
        bg.line.fill.background()
        return bg

    def add_header(slide, title_text, category_text="INT402 ACADEMIC PRESENTATION • SIH1329"):
        # Top banner background subtle line
        top_bar = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, 0, 0, prs.slide_width, Inches(0.1))
        top_bar.fill.solid()
        top_bar.fill.fore_color.rgb = ACCENT
        top_bar.line.fill.background()

        # Category / Breadcrumb
        cat_box = slide.shapes.add_textbox(Inches(0.8), Inches(0.35), Inches(11.7), Inches(0.3))
        tf_cat = cat_box.text_frame
        tf_cat.word_wrap = True
        tf_cat.margin_left = tf_cat.margin_top = tf_cat.margin_right = tf_cat.margin_bottom = 0
        p_cat = tf_cat.paragraphs[0]
        p_cat.text = category_text.upper()
        p_cat.font.size = Pt(10)
        p_cat.font.bold = True
        p_cat.font.color.rgb = ACCENT

        # Title
        title_box = slide.shapes.add_textbox(Inches(0.8), Inches(0.65), Inches(11.7), Inches(0.65))
        tf_t = title_box.text_frame
        tf_t.word_wrap = True
        tf_t.margin_left = tf_t.margin_top = tf_t.margin_right = tf_t.margin_bottom = 0
        p_t = tf_t.paragraphs[0]
        p_t.text = title_text
        p_t.font.size = Pt(22)
        p_t.font.bold = True
        p_t.font.color.rgb = PRIMARY

    def add_card(slide, left, top, width, height, title="", border_color=CARD_BORDER, bg_color=CARD_BG):
        card = slide.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, left, top, width, height)
        card.fill.solid()
        card.fill.fore_color.rgb = bg_color
        card.line.color.rgb = border_color
        card.line.width = Pt(1.5)

        if title:
            tb = slide.shapes.add_textbox(left + Inches(0.2), top + Inches(0.18), width - Inches(0.4), Inches(0.4))
            tf = tb.text_frame
            tf.word_wrap = True
            tf.margin_left = tf.margin_top = tf.margin_right = tf.margin_bottom = 0
            p = tf.paragraphs[0]
            p.text = title
            p.font.size = Pt(14)
            p.font.bold = True
            p.font.color.rgb = SECONDARY
        return card

    def add_stat_box(slide, left, top, width, height, number_str, label_str, accent_color=ACCENT):
        card = slide.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, left, top, width, height)
        card.fill.solid()
        card.fill.fore_color.rgb = WHITE
        card.line.color.rgb = CARD_BORDER
        card.line.width = Pt(1.5)

        tb = slide.shapes.add_textbox(left + Inches(0.15), top + Inches(0.15), width - Inches(0.3), height - Inches(0.3))
        tf = tb.text_frame
        tf.word_wrap = True
        tf.margin_left = tf.margin_top = tf.margin_right = tf.margin_bottom = 0
        
        p1 = tf.paragraphs[0]
        p1.text = number_str
        p1.font.size = Pt(26)
        p1.font.bold = True
        p1.font.color.rgb = accent_color
        
        p2 = tf.add_paragraph()
        p2.text = label_str
        p2.font.size = Pt(11)
        p2.font.color.rgb = TEXT_MUTED

    # -------------------------------------------------------------
    # SLIDE 1: TITLE SLIDE (Dark Navy Hero Theme)
    # -------------------------------------------------------------
    s1 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s1, PRIMARY)

    # Accent Gold Top Strip
    top_strip = s1.shapes.add_shape(MSO_SHAPE.RECTANGLE, 0, 0, prs.slide_width, Inches(0.15))
    top_strip.fill.solid()
    top_strip.fill.fore_color.rgb = ACCENT
    top_strip.line.fill.background()

    # Pill badge
    badge = s1.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(1.0), Inches(1.2), Inches(3.8), Inches(0.4))
    badge.fill.solid()
    badge.fill.fore_color.rgb = SECONDARY
    badge.line.color.rgb = ACCENT
    badge_tf = badge.text_frame
    badge_tf.margin_left = badge_tf.margin_top = badge_tf.margin_right = badge_tf.margin_bottom = 0
    bp = badge_tf.paragraphs[0]
    bp.alignment = PP_ALIGN.CENTER
    bp.text = "SMART INDIA HACKATHON • ID: SIH1329"
    bp.font.size = Pt(11)
    bp.font.bold = True
    bp.font.color.rgb = WHITE

    # Main Title
    t_box = s1.shapes.add_textbox(Inches(1.0), Inches(1.85), Inches(11.3), Inches(1.8))
    t_tf = t_box.text_frame
    t_tf.word_wrap = True
    p = t_tf.paragraphs[0]
    p.text = "Web-based Platform for Collecting 360-Degree Feedback on Government of India News Stories from Regional Media"
    p.font.size = Pt(30)
    p.font.bold = True
    p.font.color.rgb = WHITE

    # Subtitle
    sub_box = s1.shapes.add_textbox(Inches(1.0), Inches(3.8), Inches(11.3), Inches(0.8))
    sub_tf = sub_box.text_frame
    sub_tf.word_wrap = True
    sp = sub_tf.paragraphs[0]
    sp.text = "Empowering Citizen Engagement, Regional Sentiment Intelligence & Public Policy Communication Analysis"
    sp.font.size = Pt(16)
    sp.font.color.rgb = RGBColor(203, 213, 225)

    # Meta Info Card
    meta_card = s1.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(1.0), Inches(4.9), Inches(11.3), Inches(1.6))
    meta_card.fill.solid()
    meta_card.fill.fore_color.rgb = RGBColor(26, 47, 85)
    meta_card.line.color.rgb = RGBColor(51, 65, 85)
    m_tf = meta_card.text_frame
    m_tf.word_wrap = True
    m_tf.margin_left = Inches(0.4)
    m_tf.margin_top = Inches(0.2)
    
    mp1 = m_tf.paragraphs[0]
    mp1.text = "COURSE EVALUATION: INT402 – Modern Web Programming Tools and Techniques"
    mp1.font.bold = True
    mp1.font.size = Pt(13)
    mp1.font.color.rgb = ACCENT

    mp2 = m_tf.add_paragraph()
    mp2.text = "Technology Stack: ASP.NET Core 10 MVC • Entity Framework Core • Linux SQL Server 2022 • Docker • Chart.js • Bootstrap 5"
    mp2.font.size = Pt(12)
    mp2.font.color.rgb = WHITE

    mp3 = m_tf.add_paragraph()
    mp3.text = "Academic Prototype Notice: Developed strictly for academic and demonstration purposes. Not an official Government of India portal."
    mp3.font.size = Pt(11)
    mp3.font.italic = True
    mp3.font.color.rgb = RGBColor(148, 163, 184)

    s1.notes_slide.notes_text_frame.text = (
        "Good morning/afternoon respected evaluators. Today I am presenting my academic project for INT402: "
        "A Web-based Platform for Collecting 360-Degree Feedback on Government of India News Stories from Regional Media, "
        "addressing Problem Statement SIH1329. This platform bridges the vital communication gap between grassroots "
        "regional media reporting and central public policy feedback using modern .NET 10, Docker, and SQL Server."
    )

    # -------------------------------------------------------------
    # SLIDE 2: PROBLEM STATEMENT & BACKGROUND
    # -------------------------------------------------------------
    s2 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s2)
    add_header(s2, "Problem Statement & Context (SIH1329)")

    c1 = add_card(s2, Inches(0.8), Inches(1.5), Inches(5.6), Inches(5.3), "The Core Challenge")
    tb = s2.shapes.add_textbox(Inches(1.0), Inches(2.1), Inches(5.2), Inches(4.5))
    tf = tb.text_frame
    tf.word_wrap = True
    points = [
        ("Grassroots Sentiment Disconnect: ", "Central government communication is predominantly analyzed in English and Hindi national media, missing ground realities captured in regional press."),
        ("Absence of Structured Feedback: ", "Existing portals provide unidirectional press releases with no systematic mechanism for citizens to provide multi-dimensional qualitative and quantitative ratings."),
        ("Regional Disparities: ", "Policies affecting agriculture, infrastructure, or healthcare impact states differently (e.g., Punjab vs. Tamil Nadu), requiring state-level sentiment breakdown."),
        ("Information Asymmetry: ", "Citizens often struggle to find authentic regional press coverage alongside government communication.")
    ]
    for i, (head, body) in enumerate(points):
        p = tf.paragraphs[0] if i == 0 else tf.add_paragraph()
        p.space_after = Pt(12)
        r1 = p.add_run()
        r1.text = "• " + head
        r1.font.bold = True
        r1.font.size = Pt(12)
        r1.font.color.rgb = SECONDARY
        r2 = p.add_run()
        r2.text = body
        r2.font.size = Pt(11)
        r2.font.color.rgb = TEXT_DARK

    c2 = add_card(s2, Inches(6.8), Inches(1.5), Inches(5.7), Inches(5.3), "Why SIH1329 Matters")
    tb2 = s2.shapes.add_textbox(Inches(7.0), Inches(2.1), Inches(5.3), Inches(4.5))
    tf2 = tb2.text_frame
    tf2.word_wrap = True
    points2 = [
        ("Bridging Media Reception: ", "Regional newspapers and digital news outlets possess deep contextual knowledge of local citizen concerns."),
        ("360-Degree Evaluation: ", "Goes beyond simple 'like/dislike' by assessing star rating, transparency, implementation efficacy, and constructive feedback."),
        ("Dual-Layer Sentiment Analysis: ", "Cross-validates user-declared sentiment against an automated algorithmic sentiment classification engine."),
        ("Actionable Policy Insights: ", "Equips communication officers and decision-makers with state-by-state heatmaps, monthly trends, and downloadable CSV analytics.")
    ]
    for i, (head, body) in enumerate(points2):
        p = tf2.paragraphs[0] if i == 0 else tf2.add_paragraph()
        p.space_after = Pt(12)
        r1 = p.add_run()
        r1.text = "✓ " + head
        r1.font.bold = True
        r1.font.size = Pt(12)
        r1.font.color.rgb = ACCENT_GREEN
        r2 = p.add_run()
        r2.text = body
        r2.font.size = Pt(11)
        r2.font.color.rgb = TEXT_DARK

    s2.notes_slide.notes_text_frame.text = (
        "In India, regional newspapers and digital outlets have massive readership and capture nuances that national media overlooks. "
        "The problem identified in SIH1329 is that there is no structured mechanism to aggregate feedback on these regional stories. "
        "Our solution enables citizens from all 31 States and Union Territories to voice feedback across 5 dimensions."
    )

    # -------------------------------------------------------------
    # SLIDE 3: PROJECT OBJECTIVES & KEY INNOVATIONS
    # -------------------------------------------------------------
    s3 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s3)
    add_header(s3, "Project Objectives & Technical Vision")

    cards_data = [
        ("1. Centralized Regional News Discovery", "Aggregates government news stories tagged by State/UT, Regional Language (Hindi, Marathi, Tamil, Punjabi, etc.), and Governance Sector.", ACCENT),
        ("2. Multi-Dimensional 360° Review Engine", "Captures 1-to-5 star ratings, focus dimension (Implementation, Transparency, Public Impact), sentiment, and constructive suggestions.", SECONDARY),
        ("3. Automated Lexicon Sentiment Analysis", "Employs an algorithmic sentiment engine that compares declared citizen sentiment with computed text sentiment scores.", ACCENT_GREEN),
        ("4. Role-Based Citizen & Admin Portals", "Custom dashboards: Citizens track and self-edit their past feedback; Administrators moderate reviews and manage stories.", PRIMARY),
        ("5. Regional Analytics & Stored Procedures", "Real-time Chart.js visual dashboards powered by SQL Server Views and stored procedures for high-performance KPI queries.", SECONDARY),
        ("6. RESTful APIs & Open Standards", "Full Swagger/OpenAPI-documented REST APIs for external integration with government mobile apps and data portals.", ACCENT)
    ]

    for idx, (title, desc, color) in enumerate(cards_data):
        row = idx // 3
        col = idx % 3
        x = Inches(0.8 + col * 4.0)
        y = Inches(1.5 + row * 2.7)
        c = add_card(s3, x, y, Inches(3.7), Inches(2.4), title)
        
        tb = s3.shapes.add_textbox(x + Inches(0.2), y + Inches(0.7), Inches(3.3), Inches(1.5))
        tf = tb.text_frame
        tf.word_wrap = True
        p = tf.paragraphs[0]
        p.text = desc
        p.font.size = Pt(11)
        p.font.color.rgb = TEXT_DARK

    s3.notes_slide.notes_text_frame.text = (
        "Here are our 6 core project objectives. Notice how they satisfy every aspect of modern web application engineering: "
        "content aggregation, multi-dimensional user feedback, natural language processing, role-based access control, "
        "advanced relational database optimization via stored procedures, and open REST APIs."
    )

    # -------------------------------------------------------------
    # SLIDE 4: SYSTEM ARCHITECTURE & N-TIER DESIGN
    # -------------------------------------------------------------
    s4 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s4)
    add_header(s4, "N-Tier System Architecture")

    tiers = [
        ("Presentation Layer (Client)", "Razor Views (HTML5/CSS3) • Bootstrap 5.3 • Responsive Admin Layout (_AdminLayout)\nChart.js Data Visualizations • Interactive Star Rating Widgets • Vanilla JavaScript", SECONDARY),
        ("API & Gateway Layer", "ASP.NET Core 10 Web APIs • Swagger / OpenAPI Documentation • Anti-Forgery Validation\nJSON Cycle Serialization Handling (ReferenceHandler.IgnoreCycles) • Route-based RBAC", ACCENT),
        ("Business Logic Layer (Services)", "INewsService (Filtering & Pagination) • IFeedbackService (360° Review Logic)\nISentimentService (Lexicon Classifier) • IAnalyticsService (KPI Aggregation) • IDataSeeder", ACCENT_GREEN),
        ("Data Access Layer (EF Core 10)", "ApplicationDbContext • LINQ Queries • SQL Views (vw_RegionalFeedbackSummary)\nStored Procedure Executors (sp_GetDashboardKPIs, sp_GetRegionalFeedbackAnalytics)", PRIMARY),
        ("Database & Container Infrastructure", "Microsoft SQL Server 2022 Linux Container • Docker Multi-Container Network\nPersistent Volume (govfeedback360-sql-data) • Port 5189 (App) / Port 1433 (SQL Server)", SECONDARY)
    ]

    for idx, (layer_name, details, color) in enumerate(tiers):
        y = Inches(1.4 + idx * 1.1)
        card = s4.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(0.8), y, Inches(11.7), Inches(0.95))
        card.fill.solid()
        card.fill.fore_color.rgb = WHITE
        card.line.color.rgb = color
        card.line.width = Pt(2.0)

        # Pill on left
        pill = s4.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(1.0), y + Inches(0.15), Inches(3.2), Inches(0.65))
        pill.fill.solid()
        pill.fill.fore_color.rgb = color
        pill.line.fill.background()
        ptf = pill.text_frame
        ptf.margin_left = ptf.margin_top = ptf.margin_right = ptf.margin_bottom = 0
        pp = ptf.paragraphs[0]
        pp.alignment = PP_ALIGN.CENTER
        pp.text = layer_name
        pp.font.bold = True
        pp.font.size = Pt(11)
        pp.font.color.rgb = WHITE

        # Details on right
        dtb = s4.shapes.add_textbox(Inches(4.4), y + Inches(0.12), Inches(7.9), Inches(0.7))
        dtf = dtb.text_frame
        dtf.word_wrap = True
        dtf.margin_left = dtf.margin_top = dtf.margin_right = dtf.margin_bottom = 0
        dp = dtf.paragraphs[0]
        dp.text = details
        dp.font.size = Pt(10)
        dp.font.color.rgb = TEXT_DARK

    s4.notes_slide.notes_text_frame.text = (
        "The system strictly follows an N-Tier Clean Architecture. Controllers depend on interface abstractions, "
        "decoupling business logic from database interactions. Data access uses Entity Framework Core 10 with direct "
        "fallback to high-performance stored procedures. Everything is encapsulated in Linux Docker containers."
    )

    # -------------------------------------------------------------
    # SLIDE 5: DATABASE SCHEMA & DATA INTEGRITY
    # -------------------------------------------------------------
    s5 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s5)
    add_header(s5, "Relational Database Design & Schema Integrity")

    tables = [
        ("AspNetUsers", "Id (GUID PK)\nFullName, State\nLanguage, AgeGroup\nOccupation, Role\nPasswordHash (PBKDF2)"),
        ("Categories", "Id (Int PK)\nName (Unique)\nDescription\nColorHex, IconClass\nIsActive"),
        ("NewsStories", "Id (Int PK)\nTitle, Summary, Content\nMediaSource, SourceUrl\nState, Language\nCategoryId (FK)\nIsPublished, ViewCount"),
        ("Feedbacks", "Id (Int PK)\nRating (1-5 Star)\nUserSentiment\nDetectedSentiment\nComments, Suggestions\nNewsStoryId (FK), UserId (FK)\nUnique (UserId, NewsStoryId)"),
        ("AuditLogs", "Id (Int PK)\nUserId (FK)\nAction, EntityName\nEntityId, IPAddress\nTimestamp, Old/NewValues")
    ]

    for idx, (tname, cols) in enumerate(tables):
        x = Inches(0.8 + idx * 2.38)
        c = add_card(s5, x, Inches(1.5), Inches(2.25), Inches(3.4), tname)
        tb = s5.shapes.add_textbox(x + Inches(0.15), Inches(2.1), Inches(1.95), Inches(2.6))
        tf = tb.text_frame
        tf.word_wrap = True
        p = tf.paragraphs[0]
        p.text = cols
        p.font.size = Pt(10)
        p.font.color.rgb = TEXT_DARK

    # Bottom Callout: Key Integrity Rules & Stored Procedures
    callout = s5.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(0.8), Inches(5.1), Inches(11.7), Inches(1.7))
    callout.fill.solid()
    callout.fill.fore_color.rgb = RGBColor(241, 245, 249)
    callout.line.color.rgb = SECONDARY
    callout.line.width = Pt(1.5)

    ctb = s5.shapes.add_textbox(Inches(1.0), Inches(5.2), Inches(11.3), Inches(1.5))
    ctf = ctb.text_frame
    ctf.word_wrap = True
    cp1 = ctf.paragraphs[0]
    cp1.text = "Advanced Database Features & Integrity Enforcements (CO5 Compliance):"
    cp1.font.bold = True
    cp1.font.size = Pt(12)
    cp1.font.color.rgb = SECONDARY

    rules = [
        "1. Duplicate Feedback Prevention: Composite unique index on (UserId, NewsStoryId) ensures a citizen can submit only one review per story.",
        "2. Cascading Delete Protections: Categories have OnDelete: Restrict to safeguard existing news stories; Feedbacks cascade on story deletion.",
        "3. SQL Views: vw_NewsFeedbackSummary (aggregates rating counts) & vw_RegionalFeedbackSummary (state-wise sentiment counts).",
        "4. Stored Procedures: sp_GetDashboardKPIs (instant platform KPI rollups) & sp_GetRegionalFeedbackAnalytics (state satisfaction ranking)."
    ]
    for r in rules:
        p = ctf.add_paragraph()
        p.text = r
        p.font.size = Pt(10)
        p.font.color.rgb = TEXT_DARK

    s5.notes_slide.notes_text_frame.text = (
        "Data integrity is paramount. In ApplicationDbContext, we configured a composite unique index on UserId and NewsStoryId "
        "to prevent review spamming at the database engine level. We also created SQL views and stored procedures for CO5 compliance."
    )

    # -------------------------------------------------------------
    # SLIDE 6: PUBLIC PORTAL & NEWS DISCOVERY WORKFLOW
    # -------------------------------------------------------------
    s6 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s6)
    add_header(s6, "Public News Discovery & Regional Filtering")

    f_cards = [
        ("Multi-Faceted Regional Filtering", "• 31 Indian States & Union Territories dynamically supported.\n• 13 Regional Languages (Hindi, Marathi, Tamil, Punjabi, Telugu, etc.).\n• 10 Governance Sectors (Infrastructure, Digital India, Health, Agriculture, Education).\n• Live keyword search on headlines and summaries."),
        ("Story Details & AI Summaries", "• Full article reader with regional media publication source attribution.\n• Direct link to verified external regional media news sources.\n• AI-generated executive summary box for rapid citizen reading.\n• Tags and sector badges for contextual discovery."),
        ("Community Sentiment Breakdown", "• Live community metrics visible on every news story:\n  - Overall average rating out of 5 stars.\n  - Tri-color visual sentiment bar (Positive, Neutral, Negative %).\n  - Rating breakdown distribution showing exact 1-to-5 star vote counts.")
    ]

    for idx, (title, desc) in enumerate(f_cards):
        x = Inches(0.8 + idx * 4.0)
        c = add_card(s6, x, Inches(1.5), Inches(3.7), Inches(5.3), title)
        tb = s6.shapes.add_textbox(x + Inches(0.2), Inches(2.2), Inches(3.3), Inches(4.3))
        tf = tb.text_frame
        tf.word_wrap = True
        p = tf.paragraphs[0]
        p.text = desc
        p.font.size = Pt(11)
        p.font.color.rgb = TEXT_DARK

    s6.notes_slide.notes_text_frame.text = (
        "The public news discovery portal allows citizens to browse national and regional stories with zero friction. "
        "Every news card shows the originating regional outlet, such as Lokmat or Dinamalar, and displays current feedback metrics."
    )

    # -------------------------------------------------------------
    # SLIDE 7: 360-DEGREE FEEDBACK ENGINE
    # -------------------------------------------------------------
    s7 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s7)
    add_header(s7, "360-Degree Feedback Engine & Dual Sentiment")

    dims = [
        ("Dimension 1: 1-to-5 Star Interactive Rating", "Citizens rate policy effectiveness from 1 ('Very Poor') to 5 ('Excellent'). Interactive hover star selector with live label updates."),
        ("Dimension 2: User-Declared Sentiment", "Explicit sentiment selection via radio pills (Positive / Neutral / Negative) capturing the citizen's intentional sentiment."),
        ("Dimension 3: Evaluation Focus Area", "Dropdown selector targeting specific policy aspects: Implementation, Transparency, Public Impact, Clarity, or Regional Relevance."),
        ("Dimension 4: Qualitative Commentary", "Rich feedback textarea where citizens provide in-depth commentary on local impacts and news story accuracy."),
        ("Dimension 5: Constructive Suggestions", "Dedicated field for citizen-driven recommendations to help government departments refine communication and implementation.")
    ]

    for idx, (dname, ddesc) in enumerate(dims):
        y = Inches(1.5 + idx * 1.05)
        card = s7.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(0.8), y, Inches(7.5), Inches(0.92))
        card.fill.solid()
        card.fill.fore_color.rgb = WHITE
        card.line.color.rgb = CARD_BORDER
        card.line.width = Pt(1.5)

        tb = s7.shapes.add_textbox(Inches(1.0), y + Inches(0.1), Inches(7.1), Inches(0.75))
        tf = tb.text_frame
        tf.word_wrap = True
        tf.margin_left = tf.margin_top = tf.margin_right = tf.margin_bottom = 0
        p1 = tf.paragraphs[0]
        p1.text = dname
        p1.font.bold = True
        p1.font.size = Pt(11)
        p1.font.color.rgb = SECONDARY
        p2 = tf.add_paragraph()
        p2.text = ddesc
        p2.font.size = Pt(9.5)
        p2.font.color.rgb = TEXT_DARK

    # Right Card: Dual Sentiment Engine
    right_card = add_card(s7, Inches(8.6), Inches(1.5), Inches(3.9), Inches(5.2), "Dual Sentiment Architecture")
    rtb = s7.shapes.add_textbox(Inches(8.8), Inches(2.1), Inches(3.5), Inches(4.3))
    rtf = rtb.text_frame
    rtf.word_wrap = True
    rtf.margin_left = rtf.margin_top = rtf.margin_right = rtf.margin_bottom = 0
    rp = rtf.paragraphs[0]
    rp.text = (
        "The platform pairs User Sentiment with an automated SentimentService:\n\n"
        "1. Lexicon Scoring Engine:\n"
        "Scans comment text for positive indicators (e.g. 'effective', 'beneficial', 'progressive') "
        "and negative indicators (e.g. 'delayed', 'corruption', 'flawed').\n\n"
        "2. Cross-Verification:\n"
        "Compares user sentiment against detected sentiment to identify sarcasm, sentiment divergence, "
        "or misclassification.\n\n"
        "3. Policy Impact Score:\n"
        "Feeds aggregated sentiment metrics directly into the administrative analytics intelligence engine."
    )
    rp.font.size = Pt(10)
    rp.font.color.rgb = TEXT_DARK

    s7.notes_slide.notes_text_frame.text = (
        "Why is it called 360-Degree feedback? Because it captures 5 distinct dimensions: Star rating, declared sentiment, "
        "focus dimension, descriptive comments, and actionable suggestions. Crucially, we cross-validate user sentiment with "
        "an algorithmic sentiment detection service."
    )

    # -------------------------------------------------------------
    # SLIDE 8: CITIZEN DASHBOARD & SELF-SERVICE
    # -------------------------------------------------------------
    s8 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s8)
    add_header(s8, "Citizen Portal & Feedback Management")

    add_stat_box(s8, Inches(0.8), Inches(1.5), Inches(3.6), Inches(1.3), "Personal Review Hub", "Citizen Dashboard (/Citizen/Dashboard)")
    add_stat_box(s8, Inches(4.8), Inches(1.5), Inches(3.6), Inches(1.3), "Self-Editing Rights", "Full CRUD on Own Feedback")
    add_stat_box(s8, Inches(8.8), Inches(1.5), Inches(3.7), Inches(1.3), "Soft Deletion", "Reversible User Data Privacy")

    c_left = add_card(s8, Inches(0.8), Inches(3.0), Inches(5.7), Inches(3.8), "Citizen Review Tracking & History")
    ltb = s8.shapes.add_textbox(Inches(1.0), Inches(3.6), Inches(5.3), Inches(3.0))
    ltf = ltb.text_frame
    ltf.word_wrap = True
    lp = ltf.paragraphs[0]
    lp.text = (
        "• Historical Activity Table: Displays every news story the citizen has reviewed, including star ratings, dates, and sentiment badges.\n\n"
        "• Personal Rating Statistics: Computes the user's average rating given and visualizes their positive vs. critical review tendencies.\n\n"
        "• Recommended Stories: Suggests unread regional news matching the citizen's registered state and language preferences."
    )
    lp.font.size = Pt(11)
    lp.font.color.rgb = TEXT_DARK

    c_right = add_card(s8, Inches(6.8), Inches(3.0), Inches(5.7), Inches(3.8), "Self-Editing & Deletion Lifecycle")
    rtb2 = s8.shapes.add_textbox(Inches(7.0), Inches(3.6), Inches(5.3), Inches(3.0))
    rtf2 = rtb2.text_frame
    rtf2.word_wrap = True
    rp2 = rtf2.paragraphs[0]
    rp2.text = (
        "• Edit Feedback (/Citizen/EditFeedback/{id}): Allows citizens to adjust ratings or update comments as policies evolve or more information becomes known.\n\n"
        "• Ownership Authorization: Strict role and user ID validation prevents users from altering or deleting reviews submitted by others.\n\n"
        "• Profile Management (/Account/Profile): Citizens can update their home state, age group, occupation, and language preference at any time."
    )
    rp2.font.size = Pt(11)
    rp2.font.color.rgb = TEXT_DARK

    s8.notes_slide.notes_text_frame.text = (
        "Citizens have complete transparency and ownership over their data. They can revisit any feedback they previously submitted, "
        "edit their rating if government implementation changes, or soft-delete it. Their dashboard also tracks their review contributions."
    )

    # -------------------------------------------------------------
    # SLIDE 9: ADMINISTRATIVE GOVERNANCE & CONTENT MANAGEMENT
    # -------------------------------------------------------------
    s9 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s9)
    add_header(s9, "Administrative Governance & Content Management")

    admin_cols = [
        ("News Story CRUD Lifecycle", "• Create News (/Admin/CreateNews):\n  Select Category, State, Language, Source Outlet, and Headline.\n• Edit & Publish Control:\n  Toggle story visibility, draft status, and feature flags.\n• Archive & Clean Deletion:\n  Deletes stories while respecting foreign key constraints."),
        ("Feedback Moderation Queue", "• Active Moderation (/Admin/ManageFeedback):\n  Review submitted citizen comments across all news stories.\n• Sentiment & Flag Filtering:\n  Filter reviews by Positive, Neutral, Negative, or Flagged status.\n• Administrative Action:\n  Approve constructive reviews or flag toxic content with audit notes."),
        ("User & Sector Directory", "• Citizen Directory (/Admin/Users):\n  Audit registered citizens, state origin, preferred language, and registration timestamps.\n• Category Management (/Admin/Categories):\n  View governance sectors with live story count badges and visual branding.")
    ]

    for idx, (title, desc) in enumerate(admin_cols):
        x = Inches(0.8 + idx * 4.0)
        c = add_card(s9, x, Inches(1.5), Inches(3.7), Inches(5.3), title)
        tb = s9.shapes.add_textbox(x + Inches(0.2), Inches(2.2), Inches(3.3), Inches(4.3))
        tf = tb.text_frame
        tf.word_wrap = True
        p = tf.paragraphs[0]
        p.text = desc
        p.font.size = Pt(11)
        p.font.color.rgb = TEXT_DARK

    s9.notes_slide.notes_text_frame.text = (
        "Administrators have a dedicated control center. They can publish new regional stories, review feedback in a moderation queue, "
        "and inspect registered citizen demographics to ensure balanced regional representation across India."
    )

    # -------------------------------------------------------------
    # SLIDE 10: ANALYTICS INTELLIGENCE & VISUALIZATIONS
    # -------------------------------------------------------------
    s10 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s10)
    add_header(s10, "Real-Time Visual Analytics & Sentiment Intelligence")

    charts = [
        ("Sentiment Doughnut Chart", "Visualizes real-time distribution of Positive, Neutral, and Negative citizen sentiment across India or filtered by state."),
        ("Regional Satisfaction Bar Chart", "Side-by-side comparison of feedback volume and average satisfaction rating across Indian states (Delhi, Maharashtra, UP, TN, Punjab)."),
        ("Monthly Citizen Engagement Trend", "Line chart tracking chronological citizen participation and review volume over 6-month intervals."),
        ("1-to-5 Star Rating Distribution", "Detailed breakdown of public approval distribution, highlighting polarization or consensus on specific government initiatives.")
    ]

    for idx, (title, desc) in enumerate(charts):
        row = idx // 2
        col = idx % 2
        x = Inches(0.8 + col * 5.95)
        y = Inches(1.5 + row * 2.7)
        c = add_card(s10, x, y, Inches(5.75), Inches(2.4), title)
        tb = s10.shapes.add_textbox(x + Inches(0.2), y + Inches(0.7), Inches(5.3), Inches(1.5))
        tf = tb.text_frame
        tf.word_wrap = True
        p = tf.paragraphs[0]
        p.text = desc
        p.font.size = Pt(11)
        p.font.color.rgb = TEXT_DARK

    s10.notes_slide.notes_text_frame.text = (
        "The Analytics dashboard leverages Chart.js to render 4 distinct visual perspectives. All metrics are aggregated directly "
        "from SQL Server using views and stored procedures, ensuring lightning-fast performance even with large datasets."
    )

    # -------------------------------------------------------------
    # SLIDE 11: REPORTING & CSV DATA EXPORT
    # -------------------------------------------------------------
    s11 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s11)
    add_header(s11, "Reporting & Real-Time Data Export (ReportsController)")

    r_card1 = add_card(s11, Inches(0.8), Inches(1.5), Inches(5.7), Inches(5.3), "Executive Reporting Views")
    rtb1 = s11.shapes.add_textbox(Inches(1.0), Inches(2.1), Inches(5.3), Inches(4.5))
    rtf1 = rtb1.text_frame
    rtf1.word_wrap = True
    rp1 = rtf1.paragraphs[0]
    rp1.text = (
        "• Top Discussed Stories Ranking:\n"
        "  Identifies high-impact government initiatives generating the greatest volume of public reaction.\n\n"
        "• Regional Media Response Summary:\n"
        "  Aggregates total published stories, total feedback submissions, and average rating by state.\n\n"
        "• Executive Summary KPI Cards:\n"
        "  Quick glance at total active users, story counts, feedback totals, and satisfaction averages."
    )
    rp1.font.size = Pt(11)
    rp1.font.color.rgb = TEXT_DARK

    r_card2 = add_card(s11, Inches(6.8), Inches(1.5), Inches(5.7), Inches(5.3), "One-Click Downloadable CSV Exports")
    rtb2 = s11.shapes.add_textbox(Inches(7.0), Inches(2.1), Inches(5.3), Inches(4.5))
    rtf2 = rtb2.text_frame
    rtf2.word_wrap = True
    rp2 = rtf2.paragraphs[0]
    rp2.text = (
        "The ReportsController provides 3 dedicated CSV export endpoints:\n\n"
        "1. Export News Stories CSV (/Reports/ExportNewsCsv):\n"
        "   Downloads all published stories with Title, Media Source, State, Language, Category, Views, and Dates.\n\n"
        "2. Export Feedback Submissions CSV (/Reports/ExportFeedbackCsv):\n"
        "   Exports all citizen reviews including Star Rating, Declared Sentiment, Detected Sentiment, Comments, and Suggestions.\n\n"
        "3. Export Registered Users CSV (/Reports/ExportUsersCsv):\n"
        "   Exports demographic profiles (State, Preferred Language, Occupation, Age Group) while excluding sensitive password hashes."
    )
    rp2.font.size = Pt(11)
    rp2.font.color.rgb = TEXT_DARK

    s11.notes_slide.notes_text_frame.text = (
        "Reports are not just visual; they are fully exportable. We implemented ReportsController with live CSV generators "
        "so policy analysts can download the raw datasets into Microsoft Excel, Python, or Power BI for external statistical analysis."
    )

    # -------------------------------------------------------------
    # SLIDE 12: REST APIS & SWAGGER SPECIFICATION
    # -------------------------------------------------------------
    s12 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s12)
    add_header(s12, "RESTful Web APIs & Swagger/OpenAPI Documentation")

    api_groups = [
        ("News API (/api/news)", "• GET /api/news: Paginated published regional news with search & filtering.\n• GET /api/news/{id}: Single story details by identifier.\n• POST /api/news: Author new story (Admin authenticated).\n• PUT /api/news/{id}: Update news story.\n• DELETE /api/news/{id}: Remove story."),
        ("Feedback API (/api/feedback)", "• GET /api/feedback: Retrieve reviews for specific story.\n• GET /api/feedback/stats/{id}: Rating breakdown & sentiment percentages.\n• POST /api/feedback: Submit 360° review (duplicate protected).\n• PUT /api/feedback/{id}: Update user's submitted review.\n• DELETE /api/feedback/{id}: Soft-delete review."),
        ("Analytics API (/api/analytics)", "• GET /api/analytics/overview: Platform-wide summary KPI numbers.\n• GET /api/analytics/sentiment: Real-time sentiment percentage ratios.\n• GET /api/analytics/regional: State-wise feedback and ratings.\n• GET /api/analytics/monthly: 6-month chronological trend data.")
    ]

    for idx, (title, desc) in enumerate(api_groups):
        x = Inches(0.8 + idx * 4.0)
        c = add_card(s12, x, Inches(1.5), Inches(3.7), Inches(5.3), title)
        tb = s12.shapes.add_textbox(x + Inches(0.2), Inches(2.2), Inches(3.3), Inches(4.3))
        tf = tb.text_frame
        tf.word_wrap = True
        p = tf.paragraphs[0]
        p.text = desc
        p.font.size = Pt(10.5)
        p.font.color.rgb = TEXT_DARK

    s12.notes_slide.notes_text_frame.text = (
        "In addition to Razor MVC views, the platform exposes a complete RESTful Web API suite documented with Swagger/OpenAPI "
        "at /swagger. We configured ReferenceHandler.IgnoreCycles to ensure seamless JSON serialization without object cycles."
    )

    # -------------------------------------------------------------
    # SLIDE 13: PRODUCTION DEPLOYMENT & DOCKERIZATION
    # -------------------------------------------------------------
    s13 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s13)
    add_header(s13, "Production Deployment & Windows SAC Resolution")

    p1 = add_card(s13, Inches(0.8), Inches(1.5), Inches(5.7), Inches(5.3), "The Windows 11 SAC Challenge")
    ptb1 = s13.shapes.add_textbox(Inches(1.0), Inches(2.1), Inches(5.3), Inches(4.5))
    ptf1 = ptb1.text_frame
    ptf1.word_wrap = True
    pp1 = ptf1.paragraphs[0]
    pp1.text = (
        "• The Root Cause (0x800711C7):\n"
        "  Windows 11 Smart App Control (SAC) Code Integrity policy blocked bare-metal execution of locally compiled DLLs.\n\n"
        "• Strict Academic Safety Constraint:\n"
        "  Zero security policy modifications, registry edits, or Defender disabling were permitted.\n\n"
        "• The Production-Grade Solution:\n"
        "  Migrated execution into official Linux Docker containers. Inside the WSL2 Linux environment, Windows PE Code Integrity does not apply, allowing 100% stable execution."
    )
    pp1.font.size = Pt(11)
    pp1.font.color.rgb = TEXT_DARK

    p2 = add_card(s13, Inches(6.8), Inches(1.5), Inches(5.7), Inches(5.3), "Verified Docker Multi-Container Stack")
    ptb2 = s13.shapes.add_textbox(Inches(7.0), Inches(2.1), Inches(5.3), Inches(4.5))
    ptf2 = ptb2.text_frame
    ptf2.word_wrap = True
    pp2 = ptf2.paragraphs[0]
    pp2.text = (
        "• Application Container (govfeedback360-app):\n"
        "  - Built with official mcr.microsoft.com/dotnet/sdk:10.0 and aspnet:10.0.\n"
        "  - Listens on Port 5189 (http://localhost:5189).\n\n"
        "• Database Container (govfeedback360-sqlserver):\n"
        "  - Official mcr.microsoft.com/mssql/server:2022-latest.\n"
        "  - Configured with user: root for volume permissions.\n"
        "  - Backed by persistent volume govfeedback360-sql-data.\n\n"
        "• Host Protection:\n"
        "  - Zero prune commands; pre-existing containers (Jenkins, EzyEduTube) fully preserved."
    )
    pp2.font.size = Pt(11)
    pp2.font.color.rgb = TEXT_DARK

    s13.notes_slide.notes_text_frame.text = (
        "A major technical achievement of this project was overcoming the Windows 11 Smart App Control restriction. "
        "Instead of attempting risky registry bypasses, we engineered a production-grade multi-container Docker solution "
        "running ASP.NET Core 10 and Linux SQL Server 2022."
    )

    # -------------------------------------------------------------
    # SLIDE 14: COURSE OUTCOME MAPPING (INT402)
    # -------------------------------------------------------------
    s14 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s14)
    add_header(s14, "INT402 Course Outcome (CO) Mapping")

    cos = [
        ("CO1: Modern Web Architecture", "Engineered clean N-Tier ASP.NET Core 10 MVC platform with dependency injection, middleware pipeline, and responsive UI.", ACCENT_GREEN),
        ("CO2: Responsive & Accessible Frontend", "Implemented 28 Razor views and partials using Bootstrap 5.3, interactive star widgets, and Chart.js graphs.", SECONDARY),
        ("CO3: Server-side Logic & Data Services", "Engineered 5 backend services (News, Feedback, Sentiment, Analytics, Seeder) with asynchronous LINQ processing.", SECONDARY),
        ("CO4: Security & Identity Management", "Integrated ASP.NET Core Identity, PBKDF2 hashing, Role-Based Access Control (Admin/Citizen), and CSRF tokens.", SECONDARY),
        ("CO5: Database Optimization & Stored Procedures", "Designed relational schema, composite unique index, SQL views (vw_RegionalFeedbackSummary), and stored procedures.", SECONDARY),
        ("CO6: Web Services & Production Deployment", "Exposed OpenAPI/Swagger REST APIs and deployed full stack in multi-container Linux Docker environment.", ACCENT_GREEN)
    ]

    for idx, (coname, codetail, color) in enumerate(cos):
        y = Inches(1.4 + idx * 0.95)
        card = s14.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(0.8), y, Inches(11.7), Inches(0.82))
        card.fill.solid()
        card.fill.fore_color.rgb = WHITE
        card.line.color.rgb = color
        card.line.width = Pt(1.5)

        tb = s14.shapes.add_textbox(Inches(1.0), y + Inches(0.1), Inches(11.3), Inches(0.65))
        tf = tb.text_frame
        tf.word_wrap = True
        tf.margin_left = tf.margin_top = tf.margin_right = tf.margin_bottom = 0
        p = tf.paragraphs[0]
        r1 = p.add_run()
        r1.text = coname + ": "
        r1.font.bold = True
        r1.font.size = Pt(11)
        r1.font.color.rgb = color
        r2 = p.add_run()
        r2.text = codetail
        r2.font.size = Pt(10)
        r2.font.color.rgb = TEXT_DARK

    s14.notes_slide.notes_text_frame.text = (
        "This slide maps the project directly against every course outcome of INT402: Modern web architecture (CO1), "
        "responsive design (CO2), server-side services (CO3), security and identity (CO4), database stored procedures (CO5), "
        "and RESTful web services with containerized deployment (CO6)."
    )

    # -------------------------------------------------------------
    # SLIDE 15: LIVE DEMO METRICS & VERIFICATION
    # -------------------------------------------------------------
    s15 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s15)
    add_header(s15, "Live Demonstration Metrics & Verification Evidence")

    add_stat_box(s15, Inches(0.8), Inches(1.5), Inches(2.7), Inches(1.3), "15", "Seeded Regional News Stories")
    add_stat_box(s15, Inches(3.8), Inches(1.5), Inches(2.7), Inches(1.3), "51", "Real Feedback Submissions")
    add_stat_box(s15, Inches(6.8), Inches(1.5), Inches(2.7), Inches(1.3), "20", "Registered Citizens & Admin")
    add_stat_box(s15, Inches(9.8), Inches(1.5), Inches(2.7), Inches(1.3), "2.7 / 5.0", "National Average Rating")

    v_card = add_card(s15, Inches(0.8), Inches(3.0), Inches(11.7), Inches(3.8), "Live Verification Results (All HTTP 200 OK)")
    vtb = s15.shapes.add_textbox(Inches(1.0), Inches(3.6), Inches(11.3), Inches(3.0))
    vtf = vtb.text_frame
    vtf.word_wrap = True
    vp = vtf.paragraphs[0]
    vp.text = (
        "• Public Portal Home: http://localhost:5189/ (HTTP 200 OK)\n"
        "• Regional News Catalog: http://localhost:5189/News (HTTP 200 OK)\n"
        "• Story Details & 360° Review: http://localhost:5189/News/Details/1 (HTTP 200 OK)\n"
        "• Citizen Authentication: http://localhost:5189/Account/Login (HTTP 200 OK)\n"
        "• Interactive Swagger UI: http://localhost:5189/swagger/index.html (HTTP 200 OK)\n"
        "• Analytics API Overview: http://localhost:5189/api/analytics/overview (HTTP 200 OK)\n"
        "• Database Stored Procedure Execution: EXEC sp_GetDashboardKPIs & EXEC sp_GetRegionalFeedbackAnalytics verified."
    )
    vp.font.size = Pt(11)
    vp.font.color.rgb = TEXT_DARK

    s15.notes_slide.notes_text_frame.text = (
        "Here are the live metrics currently running in our Docker environment. Every endpoint has been verified with curl "
        "and returns HTTP 200 OK. 51 feedbacks across 5 states and 4 languages demonstrate the platform's multi-regional capabilities."
    )

    # -------------------------------------------------------------
    # SLIDE 16: CONCLUSION & FUTURE ROADMAP
    # -------------------------------------------------------------
    s16 = prs.slides.add_slide(blank_layout)
    set_slide_bg(s16, PRIMARY)

    # Top strip
    tstrip = s16.shapes.add_shape(MSO_SHAPE.RECTANGLE, 0, 0, prs.slide_width, Inches(0.15))
    tstrip.fill.solid()
    tstrip.fill.fore_color.rgb = ACCENT
    tstrip.line.fill.background()

    # Title
    con_t = s16.shapes.add_textbox(Inches(1.0), Inches(1.0), Inches(11.3), Inches(0.8))
    ctf = con_t.text_frame
    cp = ctf.paragraphs[0]
    cp.text = "Conclusion & Future Enhancements"
    cp.font.size = Pt(28)
    cp.font.bold = True
    cp.font.color.rgb = WHITE

    # Left card: Key Takeaways
    add_card(s16, Inches(1.0), Inches(2.0), Inches(5.4), Inches(4.7), "Key Achievements", CARD_BORDER, RGBColor(26, 47, 85))
    ltb = s16.shapes.add_textbox(Inches(1.2), Inches(2.6), Inches(5.0), Inches(3.9))
    ltf = ltb.text_frame
    ltf.word_wrap = True
    lp = ltf.paragraphs[0]
    lp.text = (
        "✓ Fully Operational Platform: Solves Problem Statement SIH1329 with a complete 360-degree regional feedback loop.\n\n"
        "✓ Zero Compilation Errors: 43 initial errors eliminated; 28 Razor views and full DI services completed.\n\n"
        "✓ Production Dockerized Stack: Deployed on official .NET 10 and SQL Server 2022 Linux containers.\n\n"
        "✓ Academic Excellence: 100% mapping to INT402 Course Outcomes (CO1 through CO6)."
    )
    lp.font.size = Pt(11)
    lp.font.color.rgb = RGBColor(226, 232, 240)

    # Right card: Future Roadmap
    add_card(s16, Inches(6.8), Inches(2.0), Inches(5.5), Inches(4.7), "Future Technology Roadmap", CARD_BORDER, RGBColor(26, 47, 85))
    rtb = s16.shapes.add_textbox(Inches(7.0), Inches(2.6), Inches(5.1), Inches(3.9))
    rtf = rtb.text_frame
    rtf.word_wrap = True
    rp = rtf.paragraphs[0]
    rp.text = (
        "1. AI Large Language Model Integration:\n"
        "   Deploy Indic-BERT / Bhashini API for automated multilingual translation across all 22 scheduled Indian languages.\n\n"
        "2. Voice-to-Text Feedback:\n"
        "   Allow rural citizens to speak their feedback in regional dialects.\n\n"
        "3. WhatsApp / Telegram Bot Integration:\n"
        "   Expand feedback collection via popular messaging channels.\n\n"
        "4. Automated Fact-Checking Verification:\n"
        "   Cross-reference regional media claims against official PIB Fact Check database."
    )
    rp.font.size = Pt(11)
    rp.font.color.rgb = RGBColor(226, 232, 240)

    s16.notes_slide.notes_text_frame.text = (
        "In conclusion, SIH1329 demonstrates how modern web programming tools (.NET 10, EF Core, Docker, SQL Server) "
        "can be applied to solve an essential national governance communication challenge. In the future, we plan to integrate "
        "Bhashini AI translation and voice input. Thank you, and I am now open to your questions."
    )

    out_path = "SIH1329_Project_Presentation.pptx"
    prs.save(out_path)
    print(f"Presentation successfully saved to {out_path} ({len(prs.slides)} slides)")

if __name__ == "__main__":
    create_deck()
