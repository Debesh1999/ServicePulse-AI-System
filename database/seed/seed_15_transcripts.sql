-- ============================================================
-- ServicePulse AI System — Seed Data (15 Fictional Transcripts)
-- Generated for SQL Server (LocalDB)
-- Run against an empty ServicePulseDb after migrations
-- ============================================================

SET NOCOUNT ON;
BEGIN TRANSACTION;

-- ──────────────────────────────────────────────────────────────
-- 1. APPLICATION USERS
-- ──────────────────────────────────────────────────────────────
DECLARE @AdminUserId   UNIQUEIDENTIFIER = NEWID();
DECLARE @ManagerUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @AgentUserId   UNIQUEIDENTIFIER = NEWID();

INSERT INTO ApplicationUsers (Id, Email, PasswordHash, DisplayName, Role, IsActive, CreatedAtUtc)
VALUES
  (@AdminUserId,   'admin@servicepulse.local',          'HASH_PLACEHOLDER_admin', 'System Admin',  'Admin',   1, DATEADD(MONTH, -6, GETUTCDATE())),
  (@ManagerUserId, 'priya.sharma@servicepulse.local',   'HASH_PLACEHOLDER_priya', 'Priya Sharma',  'Manager', 1, DATEADD(MONTH, -5, GETUTCDATE())),
  (@AgentUserId,   'alice.johnson@servicepulse.local',  'HASH_PLACEHOLDER_alice', 'Alice Johnson', 'Agent',   1, DATEADD(MONTH, -4, GETUTCDATE()));

-- ──────────────────────────────────────────────────────────────
-- 2. AGENTS
-- ──────────────────────────────────────────────────────────────
DECLARE @AgentAlice   UNIQUEIDENTIFIER = NEWID();
DECLARE @AgentBob     UNIQUEIDENTIFIER = NEWID();
DECLARE @AgentChandra UNIQUEIDENTIFIER = NEWID();
DECLARE @AgentDiana   UNIQUEIDENTIFIER = NEWID();

INSERT INTO Agents (Id, UserId, EmployeeCode, FullName, TeamName)
VALUES
  (@AgentAlice,   @AgentUserId, 'A001', 'Alice Johnson',    'Tier 1 Support'),
  (@AgentBob,     NULL,         'A002', 'Bob Martinez',     'Tier 1 Support'),
  (@AgentChandra, NULL,         'A003', 'Chandra Patel',    'Billing & Payments'),
  (@AgentDiana,   NULL,         'A004', 'Diana Rosenberg',  'Technical Escalations');

-- ──────────────────────────────────────────────────────────────
-- 3. CUSTOMERS
-- ──────────────────────────────────────────────────────────────
DECLARE @Cust1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cust2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cust3 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cust4 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cust5 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cust6 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cust7 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cust8 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Customers (Id, ExternalReference, DisplayName, ContactMasked)
VALUES
  (@Cust1, 'CUST-1001', 'James Whitfield',  'j***@email.com'),
  (@Cust2, 'CUST-1002', 'Maria Gonzalez',   '+1-555-***-2847'),
  (@Cust3, 'CUST-1003', 'Haruto Tanaka',    'h***@corp.jp'),
  (@Cust4, 'CUST-1004', 'Sarah O''Brien',   '+1-555-***-9031'),
  (@Cust5, 'CUST-1005', 'Raj Mehta',        'r***@business.in'),
  (@Cust6, 'CUST-1006', 'Emily Chen',       '+1-555-***-6618'),
  (@Cust7, 'CUST-1007', 'Kwame Asante',     'k***@enterprise.gh'),
  (@Cust8, 'CUST-1008', 'Olivia Petrova',   '+44-20-***-4421');

-- ──────────────────────────────────────────────────────────────
-- 4. CALLS (15 calls)
-- ──────────────────────────────────────────────────────────────
DECLARE @Call01 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call02 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call03 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call04 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call05 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call06 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call07 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call08 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call09 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call10 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call11 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call12 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call13 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call14 UNIQUEIDENTIFIER = NEWID();
DECLARE @Call15 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Calls (Id, AgentId, CustomerId, ExternalCallId, StartedAtUtc, DurationSeconds, Status, ResolutionStatus, CreatedAtUtc)
VALUES
  (@Call01, @AgentAlice,   @Cust1, 'CALL-001', DATEADD(DAY, -15, GETUTCDATE()), 487,  'Completed', 'Resolved',  DATEADD(DAY, -15, GETUTCDATE())),
  (@Call02, @AgentBob,     @Cust2, 'CALL-002', DATEADD(DAY, -14, GETUTCDATE()), 723,  'Completed', 'Resolved',  DATEADD(DAY, -14, GETUTCDATE())),
  (@Call03, @AgentAlice,   @Cust3, 'CALL-003', DATEADD(DAY, -13, GETUTCDATE()), 945,  'Completed', 'Escalated', DATEADD(DAY, -13, GETUTCDATE())),
  (@Call04, @AgentBob,     @Cust4, 'CALL-004', DATEADD(DAY, -12, GETUTCDATE()), 362,  'Completed', 'Resolved',  DATEADD(DAY, -12, GETUTCDATE())),
  (@Call05, @AgentChandra, @Cust5, 'CALL-005', DATEADD(DAY, -11, GETUTCDATE()), 612,  'Completed', 'Open',      DATEADD(DAY, -11, GETUTCDATE())),
  (@Call06, @AgentAlice,   @Cust6, 'CALL-006', DATEADD(DAY, -10, GETUTCDATE()), 183,  'Completed', 'Resolved',  DATEADD(DAY, -10, GETUTCDATE())),
  (@Call07, @AgentDiana,   @Cust7, 'CALL-007', DATEADD(DAY,  -9, GETUTCDATE()), 834,  'Completed', 'Resolved',  DATEADD(DAY,  -9, GETUTCDATE())),
  (@Call08, @AgentBob,     @Cust8, 'CALL-008', DATEADD(DAY,  -8, GETUTCDATE()), 445,  'Completed', 'Open',      DATEADD(DAY,  -8, GETUTCDATE())),
  (@Call09, @AgentChandra, @Cust1, 'CALL-009', DATEADD(DAY,  -7, GETUTCDATE()), 556,  'Completed', 'Resolved',  DATEADD(DAY,  -7, GETUTCDATE())),
  (@Call10, @AgentAlice,   @Cust2, 'CALL-010', DATEADD(DAY,  -6, GETUTCDATE()), 398,  'Completed', 'Resolved',  DATEADD(DAY,  -6, GETUTCDATE())),
  (@Call11, @AgentDiana,   @Cust4, 'CALL-011', DATEADD(DAY,  -5, GETUTCDATE()), 678,  'Completed', 'Escalated', DATEADD(DAY,  -5, GETUTCDATE())),
  (@Call12, @AgentChandra, @Cust6, 'CALL-012', DATEADD(DAY,  -4, GETUTCDATE()), 521,  'Completed', 'Resolved',  DATEADD(DAY,  -4, GETUTCDATE())),
  (@Call13, @AgentBob,     @Cust5, 'CALL-013', DATEADD(DAY,  -3, GETUTCDATE()), 290,  'Analyzing', 'Unknown',   DATEADD(DAY,  -3, GETUTCDATE())),
  (@Call14, @AgentDiana,   @Cust8, 'CALL-014', DATEADD(DAY,  -1, GETUTCDATE()), 412,  'Queued',     NULL,        DATEADD(DAY,  -1, GETUTCDATE())),
  (@Call15, @AgentAlice,   @Cust7, 'CALL-015', DATEADD(DAY,  -2, GETUTCDATE()), 195,  'Failed',    'Unknown',   DATEADD(DAY,  -2, GETUTCDATE()));

-- ──────────────────────────────────────────────────────────────
-- 5. CALL TRANSCRIPTS (15 transcripts — one per call)
-- ──────────────────────────────────────────────────────────────

-- Transcript 1: Billing dispute — double charge refund
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call01,
'Agent: Thank you for calling ServicePulse support, my name is Alice. How can I help you today?
Customer: Hi Alice, my name is James Whitfield. I''m calling because I noticed a double charge on my last invoice — $149.99 appeared twice for the same subscription.
Agent: I''m sorry to hear about that, James. Let me pull up your account right away. Can you confirm the last four digits of the card on file?
Customer: Sure, it''s 7823.
Agent: Perfect. I can see the duplicate charge from November 3rd. It looks like our billing system processed the renewal twice. I sincerely apologize for the inconvenience.
Customer: That''s frustrating. I''ve been a loyal customer for three years.
Agent: And we truly value your loyalty, James. I''m going to issue a full refund of $149.99 right now. You should see it reflected in 3-5 business days. I''m also adding a $20 courtesy credit to your next billing cycle for the trouble.
Customer: Oh wow, that''s actually more than I expected. Thank you so much, Alice!
Agent: You''re very welcome. Is there anything else I can help with today?
Customer: No, that''s everything. You''ve been wonderful.
Agent: Thank you, James. Have a great day!',
'ManualUpload', 'en', 188, GETUTCDATE());

-- Transcript 2: Router troubleshooting
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call02,
'Agent: ServicePulse technical support, this is Bob. How can I assist you?
Customer: Hi Bob, I''m Maria Gonzalez. My internet has been dropping out every 20 minutes since yesterday afternoon. I work from home and this is really affecting my job.
Agent: I completely understand how disruptive that must be, Maria. Let''s get this resolved as quickly as possible. Are you connected via Wi-Fi or Ethernet?
Customer: Wi-Fi. My laptop is about 15 feet from the router.
Agent: Okay. First, can you check if the router lights are all solid green right now?
Customer: The power light is green, but the internet light is blinking orange.
Agent: That blinking orange typically indicates an intermittent connection to our network. Let me run a line diagnostic from our end... I can see some packet loss on your connection. Let''s try a full power cycle. Can you unplug the router, wait 30 seconds, and plug it back in?
Customer: Okay, doing that now... [pause] ... Alright, it''s booting back up. The lights are cycling.
Agent: Great. While it reconnects, I''m also pushing a firmware update to your router — version 4.2.1. This fixes a known issue with the exact chipset in your model that can cause periodic drops.
Customer: Oh, so it''s a known bug?
Agent: Yes, it was identified about two weeks ago and the fix was released last week. Your router should have auto-updated, but it seems it didn''t pick it up. I''m pushing it manually now.
Customer: Okay, all lights are green now and the internet light is solid.
Agent: Excellent. The firmware update will finish installing in about 10 minutes and the router will reboot one more time automatically. After that, the drops should stop. Can you monitor it for the rest of today and call us back if it happens again?
Customer: Will do. Thank you so much, Bob. I really appreciate the quick help.
Agent: My pleasure, Maria. Have a great rest of your workday!',
'ManualUpload', 'en', 279, GETUTCDATE());

-- Transcript 3: Angry customer — cancellation threat
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call03,
'Agent: Thank you for calling ServicePulse, this is Alice. How can I help you?
Customer: Yeah, this is Haruto Tanaka. I want to cancel my entire account. I am done with this company.
Agent: I''m sorry to hear you''re considering cancellation, Haruto. Can you tell me what''s happened so I can see if there''s anything I can do?
Customer: I''ve called THREE times about the same issue — my enterprise dashboard keeps timing out whenever I try to export reports over 10,000 rows. Each time I call, someone tells me it''s been escalated, and nothing happens. It''s been six weeks!
Agent: That is absolutely unacceptable, and I sincerely apologize for the repeated frustration. Let me review the history on your account... I can see tickets #4892, #5103, and #5244 — all related to the same export timeout issue.
Customer: Exactly. And every time it''s the same runaround. I''m paying $899 a month for the enterprise plan and I can''t even use basic functionality.
Agent: You''re completely right to be upset. Here''s what I''m going to do differently. I''m escalating this directly to our engineering team lead with a priority-one flag, which means it gets attention within 4 business hours, not the usual queue. I''m also going to stay personally assigned to this ticket so you have one point of contact.
Customer: I''ve heard promises before.
Agent: I understand your skepticism. Let me give you my direct extension — 4417 — and I will call YOU with an update by end of business tomorrow, whether or not engineering has a fix yet. You deserve to know the status without having to chase us.
Customer: Fine. But if I don''t hear from you by tomorrow, I''m going to our procurement team to switch providers.
Agent: That''s fair. You will hear from me. I''m also applying a 50% credit on this month''s invoice given the length of time this has been unresolved. Is there anything else?
Customer: No. Just fix it.
Agent: I will, Haruto. Thank you for your patience and I''ll speak with you tomorrow.',
'ManualUpload', 'en', 302, GETUTCDATE());

-- Transcript 4: New customer account setup
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call04,
'Agent: ServicePulse support, Bob speaking. How can I help?
Customer: Hi Bob, I''m Sarah O''Brien. I just signed up for the Professional plan yesterday and I''m having trouble setting up my team workspace.
Agent: Welcome aboard, Sarah! Congratulations on getting started. What seems to be the issue?
Customer: When I try to invite team members, I get an error that says ''Organization setup required.'' But I thought I already did that during signup.
Agent: Ah, I see what happened. The organization setup has two steps — the first is creating the org during signup, which you completed. But there''s a second step where you need to verify your business domain. Let me walk you through it.
Customer: Oh okay, I must have missed that.
Agent: No worries, it''s a common one! Go to Settings, then Organization, then Domain Verification. You''ll see a TXT record you need to add to your DNS.
Customer: I see it. It says I need to add a TXT record with value ''sp-verify=abc123xyz.''
Agent: Exactly. Once you add that to your domain''s DNS settings, it usually verifies within 15-30 minutes. After that, you''ll be able to invite unlimited team members.
Customer: Got it. I''ll have our IT guy add that right away. Thanks, Bob!
Agent: You''re welcome, Sarah! And if you have any other questions getting set up, don''t hesitate to call. We want to make sure your onboarding is smooth.
Customer: Appreciate it. Bye!
Agent: Bye, Sarah!',
'ManualUpload', 'en', 213, GETUTCDATE());

-- Transcript 5: Refund request — partially denied
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call05,
'Agent: Billing support, this is Chandra. How may I assist you?
Customer: Hello Chandra, this is Raj Mehta. I want a refund for the last three months of service. My team has barely used the platform.
Agent: I understand, Raj. Let me review your account usage and billing history. Can you tell me more about why the platform hasn''t been used?
Customer: We went through a company restructuring and most of the team that was using it got reassigned. Nobody told me to cancel the subscription.
Agent: I see. Looking at your account, I can confirm usage dropped significantly starting September. However, I need to be transparent with you — our refund policy covers the current billing period only. For previous months where the subscription was active and accessible, we''re unable to issue retroactive refunds.
Customer: That doesn''t seem fair. We weren''t even using it.
Agent: I understand your frustration, Raj. What I can do is the following: First, I''ll process a full refund for the current month since we''re only 5 days in. Second, I can downgrade your plan to our free tier immediately so you stop incurring charges. Third, I''ll flag your account so that if you decide to come back, you''ll get a 30% discount on your first three months.
Customer: I mean, I was hoping for more, but I suppose that''s something. Can I speak to a manager about getting at least one more month refunded?
Agent: Of course. I''m going to note everything we discussed and escalate this to my team lead. You should expect a callback within 48 hours. Would that work?
Customer: Yes, that''s fine. Please make sure they have all the context.
Agent: Absolutely. I''ve documented everything. Is there anything else, Raj?
Customer: No, thank you, Chandra. I appreciate you being straightforward about the policy.
Agent: Thank you for understanding, Raj. We''ll be in touch soon.',
'ManualUpload', 'en', 270, GETUTCDATE());

-- Transcript 6: Password reset
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call06,
'Agent: ServicePulse support, Alice here. How can I help?
Customer: Hi, this is Emily Chen. I need to reset my password. I''ve tried the forgot password link three times and I''m not getting the email.
Agent: Hi Emily! Let me help with that. Can you confirm the email address on your account?
Customer: It''s emily.chen@techforward.io.
Agent: Got it. I can see the reset emails were sent, but they might be going to your spam or junk folder. Have you checked there?
Customer: Oh... let me look. [pause] Oh! There they are, in the junk folder. I feel silly now.
Agent: Don''t feel silly at all — it happens all the time! I''d recommend adding noreply@servicepulse.com to your contacts so future emails come through properly.
Customer: Good idea, doing that now. And I just reset my password. All good!
Agent: Wonderful! Glad that was an easy fix. Anything else I can help with?
Customer: Nope, that''s it. Thanks, Alice!
Agent: Have a great day, Emily!',
'ManualUpload', 'en', 163, GETUTCDATE());

-- Transcript 7: Enterprise service outage
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call07,
'Agent: Technical escalations, Diana speaking. How can I help?
Customer: This is Kwame Asante from DataBridge Corp. Your platform has been completely down for our entire organization for the past two hours. We have 200 users unable to work. This is costing us thousands of dollars per hour.
Agent: Kwame, I understand this is a critical situation. Let me check our system status immediately. Can you provide your organization ID?
Customer: It''s ORG-7842.
Agent: Thank you. I can see your organization is on our US-East cluster. We are currently experiencing a partial outage on that cluster affecting approximately 340 organizations. Our engineering team has been working on it since 8:47 AM UTC. The issue is related to a database failover that didn''t complete correctly.
Customer: Two hours! And we weren''t even notified?
Agent: You should have received an incident notification at 9:05 AM UTC. Let me verify your notification settings... I see that your admin contact email was changed last month to kwame.asante@databridge.com — is that correct?
Customer: Yes, that''s my new email, but I never got anything.
Agent: It appears the email change didn''t propagate to our incident notification system. That''s a bug on our end and I''m filing an internal report on it now. I sincerely apologize for the communication gap.
Customer: When will the service be back?
Agent: Based on the engineering team''s latest update from 12 minutes ago, the estimated restoration time is within the next 30-45 minutes. They''ve identified the root cause and are executing the recovery procedure. I will send you a direct SMS update the moment the service is restored, and our post-incident report will be published within 24 hours.
Customer: Fine. But we need to discuss SLA credits for this downtime.
Agent: Absolutely. Under your Enterprise SLA, you are entitled to downtime credits. Once the incident is fully resolved and documented, our SLA team will automatically calculate and apply the credit to your next invoice. I''ll also personally ensure your notification settings are corrected today.
Customer: Alright. Thank you for being direct about it, Diana.
Agent: Of course, Kwame. I''ll text you the moment we''re back online.',
'ManualUpload', 'en', 302, GETUTCDATE());

-- Transcript 8: Feature request
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call08,
'Agent: ServicePulse support, this is Bob. How can I help you today?
Customer: Hi Bob, this is Olivia Petrova. I''m not calling about a problem exactly — I have a feature suggestion. Is this the right place?
Agent: Absolutely, Olivia! We love hearing from our users. What''s on your mind?
Customer: We use ServicePulse for our entire 50-person team and one thing we really miss is the ability to create custom dashboard widgets. Right now, the dashboard only shows preset metrics but we need to track KPIs specific to our business.
Agent: That''s a great suggestion. Can you give me a specific example of what you''d want to see?
Customer: Sure. We''d love a widget that shows our average resolution time broken down by product category, and another one that tracks customer satisfaction trends over the past 90 days with the ability to filter by agent.
Agent: Those are really thoughtful use cases. I''m going to log this as a formal feature request in our product feedback system. I want you to know that our product team reviews these every two weeks during sprint planning.
Customer: That''s great. Is there any way I can know if it gets picked up?
Agent: Yes! I''ll tag your request with your account email and you''ll receive an automated update if the status changes to ''Under Review'' or ''In Development.'' I can also add you to our beta program — when we do build new dashboard features, beta users get early access.
Customer: I''d love that! Sign me up.
Agent: Done! You''re now enrolled in the beta program. Anything else?
Customer: No, that''s everything. Thanks for actually listening, Bob.
Agent: That''s what we''re here for, Olivia. Your feedback genuinely helps shape the product. Have a wonderful day!',
'ManualUpload', 'en', 253, GETUTCDATE());

-- Transcript 9: GDPR data portability request
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call09,
'Agent: Billing and accounts, Chandra speaking. How can I help?
Customer: Hi Chandra, James Whitfield here. I need to understand what personal data you store about me and I want to exercise my right to data portability under GDPR.
Agent: Of course, James. We take data privacy very seriously. Let me walk you through what we have on file.
Customer: Please.
Agent: For your account, we store your name, email address, billing address, phone number, and payment card details — though the card is tokenized, meaning we don''t store the full number. We also retain your support interaction history, product usage logs, and any files you''ve uploaded to the platform.
Customer: And how long do you keep it?
Agent: Active account data is retained for the duration of your subscription plus 90 days after cancellation. Support interaction logs are retained for 2 years. Billing records are retained for 7 years as required by financial regulations. Usage analytics are anonymized after 18 months.
Customer: Okay. I want an export of all my data in a machine-readable format.
Agent: Absolutely. I''m initiating a Subject Access Request right now. Under our GDPR process, you''ll receive a secure download link within 72 hours — usually it''s faster. The export will be in JSON format and will include all personal data categories I mentioned. You''ll also receive a cover letter detailing each data category.
Customer: Perfect. And if I decide to delete my account later?
Agent: You can submit a Right to Erasure request at any time. We''ll delete all personal data within 30 days, except what we''re legally required to retain. I can email you our full privacy policy and data retention schedule for reference.
Customer: Yes, please do that. Thank you, Chandra. Very thorough.
Agent: My pleasure, James. You''ll get the SAR export link and the privacy documents within the next few days. Anything else?
Customer: That''s all. Bye.
Agent: Goodbye, James!',
'ManualUpload', 'en', 275, GETUTCDATE());

-- Transcript 10: Service upgrade
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call10,
'Agent: ServicePulse support, Alice speaking.
Customer: Hi Alice, Maria Gonzalez again. I called last week about my internet issues — everything''s been stable since, by the way.
Agent: Great to hear that, Maria! Glad the firmware update did the trick. What can I help you with today?
Customer: I''m interested in upgrading my plan. I''m currently on the 100 Mbps plan but I''ve started doing a lot of video conferencing and large file transfers for work. What are my options?
Agent: Perfect timing, actually! We just launched new speed tiers last month. From your 100 Mbps plan, you can upgrade to 300 Mbps at $59.99/month, 500 Mbps at $79.99/month, or our Gigabit plan at $99.99/month. All plans include unlimited data.
Customer: How much am I paying now?
Agent: You''re currently at $44.99/month on the 100 Mbps plan.
Customer: I think 300 would probably be enough. What''s the process?
Agent: I can process the upgrade right now. It takes effect on your next billing cycle — which is December 1st — so there''s no prorated charge. Your router already supports the higher speed, so no equipment change is needed.
Customer: That easy? Let''s do the 300 Mbps plan then.
Agent: Done! I''ve upgraded your account to the 300 Mbps plan starting December 1st. You''ll receive a confirmation email shortly. On December 1st, you may want to restart your router to ensure it picks up the new speed profile.
Customer: Wonderful. Thanks again, Alice. You''re always so helpful!
Agent: Thank you, Maria! Enjoy the faster speeds!',
'ManualUpload', 'en', 232, GETUTCDATE());

-- Transcript 11: Complaint about agent behavior
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call11,
'Agent: Technical escalations, Diana speaking.
Customer: Hi Diana, this is Sarah O''Brien. I need to file a complaint about a previous interaction with one of your agents.
Agent: I''m sorry to hear that, Sarah. Can you tell me what happened?
Customer: I called yesterday about a data sync issue and spoke with someone — I didn''t catch their name. They were dismissive, kept interrupting me while I was explaining the problem, and eventually just told me to ''try restarting the app'' without actually investigating anything. The issue is still not resolved.
Agent: That''s not the experience we want any customer to have, and I sincerely apologize. I''m going to do two things: first, address your technical issue right away, and second, formally document this complaint. Do you happen to remember approximately what time you called yesterday?
Customer: It was around 2:30 PM Eastern.
Agent: Thank you, I can look up the interaction. Now, let''s tackle your data sync issue. Can you describe what''s happening?
Customer: When I sync my workspace from the desktop app to the cloud, about 30% of the files show as ''conflict'' even though nobody else is editing them. It started after the last update.
Agent: That sounds like the sync conflict bug in version 8.4.2. There''s a known issue where the timestamp comparison uses local time instead of UTC, causing false conflicts. The fix is in version 8.4.3 which was released yesterday. Let me guide you through the update.
Customer: Okay.
Agent: Go to Help menu, then Check for Updates. Do you see version 8.4.3 available?
Customer: Yes, it''s downloading now.
Agent: Once it installs, restart the app and trigger a full re-sync by going to Settings, Sync, and clicking ''Reset Sync State.'' That will clear the false conflicts.
Customer: Alright, update is installed. Restarting now... and doing the reset sync... [pause] ... It''s syncing and — yes! No more false conflicts. Everything shows as up to date.
Agent: Excellent! I''m glad we got that resolved. Regarding your complaint, I''ve logged it with reference number CMP-2891. Our quality assurance team will review the recorded interaction from yesterday and follow up with appropriate action. You''ll receive an email confirmation of the complaint within 24 hours.
Customer: Thank you, Diana. I appreciate you actually solving the problem AND taking the complaint seriously.
Agent: Absolutely, Sarah. Both are equally important to us. Is there anything else?
Customer: No, that''s everything. Thanks again.',
'ManualUpload', 'en', 348, GETUTCDATE());

-- Transcript 12: Subscription cancellation — retained
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call12,
'Agent: Billing support, Chandra here. How can I help?
Customer: Hi, Emily Chen. I''d like to cancel my subscription.
Agent: I''m sorry to hear that, Emily. Before I process the cancellation, would you mind sharing what''s prompting the decision? Your feedback helps us improve.
Customer: Honestly, I''m just not using it enough to justify the cost. I log in maybe once a week.
Agent: I understand. Looking at your account, you''re on our Standard plan at $29.99/month. I notice you''ve been primarily using the reporting features. Did you know we have a Lite plan at $9.99/month that includes unlimited reporting? You''d just lose the advanced automation features.
Customer: Wait, really? I didn''t know there was a cheaper option. I mainly care about the reports.
Agent: Absolutely. The Lite plan gives you full access to all reporting dashboards, data exports, and up to 5 scheduled reports. The main things you''d lose are workflow automation, API access, and priority support.
Customer: I don''t use any of those. Can you switch me to the Lite plan instead?
Agent: Of course! I''ve downgraded your account to the Lite plan effective immediately. Your next bill on December 8th will be $9.99. All your existing reports and saved dashboards will carry over.
Customer: That''s perfect. I''m glad I called instead of just hitting cancel online.
Agent: Me too, Emily! And if your needs ever change, you can upgrade back anytime with no penalty. Is there anything else?
Customer: Nope, I''m all set. Thanks, Chandra!
Agent: Have a great day, Emily!',
'ManualUpload', 'en', 238, GETUTCDATE());

-- Transcript 13: Follow-up on delayed escalation
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call13,
'Agent: ServicePulse support, Bob here.
Customer: Hi Bob, Raj Mehta again. I''m following up on my refund request from last week. I was told a manager would call me within 48 hours but it''s been five days.
Agent: I apologize for the delay, Raj. Let me check the status of your escalation... I can see it was assigned to our billing team lead but it hasn''t been actioned yet. That''s not acceptable.
Customer: I''m really losing patience here.
Agent: I completely understand. I''m going to escalate this again with an urgent flag and CC our operations manager directly. I''m also sending you an email right now with the escalation reference number so you have documentation.
Customer: When can I realistically expect a response?
Agent: I''m going to push for end of business today. If you don''t hear from us by 5 PM your time, please email me directly at bob.m@servicepulse.local and I''ll personally follow up first thing tomorrow morning.
Customer: Okay, I''ll hold you to that.
Agent: Please do. I''m sorry about the delay, Raj.',
'ManualUpload', 'en', 170, GETUTCDATE());

-- Transcript 14: Data migration encoding issue
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call14,
'Agent: Technical escalations, Diana speaking.
Customer: Hi Diana, Olivia Petrova here from NovaTech Solutions. We just migrated our data from a competitor platform to ServicePulse using your import tool, but about 2,000 of our 15,000 records are showing encoding errors — special characters like accented names are displaying as question marks.
Agent: Thank you for reporting this, Olivia. Character encoding issues during migration are something we''ve seen before, usually when the source data was exported in a different encoding than UTF-8. Can you tell me what format the export was in?
Customer: It was a CSV file exported from our old platform. I''m not sure about the encoding.
Agent: That''s likely the issue. Most European language characters need UTF-8 encoding to display correctly. Here''s what we can do: I''ll create a support ticket to have our data migration specialist review and re-import the affected records with proper encoding detection. In the meantime, your other 13,000 records are fine and fully usable.
Customer: How long will the re-import take?
Agent: Typically 1-2 business days for the specialist to run the conversion and verify the results. I''ll set this as high priority given the volume. You''ll receive a notification once it''s complete, and we''ll ask you to spot-check a sample of the corrected records.
Customer: That works. Thank you, Diana.
Agent: You''re welcome, Olivia. I''ll send you the ticket number via email shortly.',
'Import', 'en', 213, GETUTCDATE());

-- Transcript 15: User limit inquiry
INSERT INTO CallTranscripts (Id, CallId, Text, Source, LanguageCode, WordCount, CreatedAtUtc)
VALUES (NEWID(), @Call15,
'Agent: ServicePulse support, Alice here.
Customer: Hi Alice, Kwame Asante. Quick question — I need to add three new admin users to our enterprise account but the admin panel says I''ve reached my user limit.
Agent: Hi Kwame! Let me check your plan details. You''re on the Enterprise 200 plan which allows up to 200 active users. Your current count is... 200 exactly.
Customer: Ah, so we''ve maxed out. Can we add more?
Agent: Yes! You have two options. You can upgrade to Enterprise Unlimited for an additional $299/month, or you can purchase user packs of 50 additional seats for $149/month each.
Customer: I think 50 more seats would cover us. Let me talk to my CFO and I''ll call back. Can you send me the pricing details in an email?
Agent: Absolutely, I''ll send that to your registered email right away, including a comparison sheet of both options. You''ll have it within the hour.
Customer: Perfect, thanks Alice.
Agent: You''re welcome, Kwame! Talk soon.',
'ManualUpload', 'en', 159, GETUTCDATE());

-- ──────────────────────────────────────────────────────────────
-- 6. ANALYSIS RESULTS (12 results — 11 Succeeded, 1 Failed)
-- ──────────────────────────────────────────────────────────────
INSERT INTO AnalysisResults (Id, CallId, AnalyzerVersion, Sentiment, RiskLevel, QualityScore, Summary, DetectedKeywordsJson, AnalysisStatus, ErrorMessage, CreatedAtUtc)
VALUES
  (NEWID(), @Call01, 'ServicePulse-AI-v1.2.0', 'Positive', 'Low',    92.50,
   'Customer reported a duplicate billing charge. Agent identified the issue quickly, issued a refund, and offered a courtesy credit. Excellent empathy and resolution speed.',
   '["double charge","refund","loyalty","courtesy credit","billing"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call02, 'ServicePulse-AI-v1.2.0', 'Positive', 'Low',    88.75,
   'Customer reported intermittent internet outages. Agent performed systematic troubleshooting, identified a known firmware issue, and resolved it remotely. Strong technical knowledge demonstrated.',
   '["internet dropping","Wi-Fi","firmware update","packet loss","power cycle","router"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call03, 'ServicePulse-AI-v1.2.0', 'Negative', 'High',   71.00,
   'Repeat caller (3rd contact) threatening cancellation due to unresolved enterprise dashboard export timeout issue. Agent handled the escalation well but underlying service failure is a churn risk. P1 escalation initiated.',
   '["cancellation","escalation","enterprise","export timeout","repeated issue","churn risk","credit"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call04, 'ServicePulse-AI-v1.2.0', 'Positive', 'Low',    90.25,
   'New customer needed help with team workspace domain verification. Agent provided clear step-by-step guidance. Good onboarding experience.',
   '["onboarding","domain verification","team workspace","organization setup","new customer"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call05, 'ServicePulse-AI-v1.2.0', 'Neutral',  'Medium', 79.50,
   'Customer requested 3-month retroactive refund due to low usage after company restructuring. Agent correctly applied policy, offered partial refund + downgrade + future discount. Escalated for further review.',
   '["refund","retroactive","restructuring","policy","downgrade","escalation"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call06, 'ServicePulse-AI-v1.2.0', 'Positive', 'Low',    94.00,
   'Simple password reset issue — emails were in spam folder. Resolved quickly with proactive tip to whitelist sender. Excellent efficiency.',
   '["password reset","email","spam folder","quick resolution"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call07, 'ServicePulse-AI-v1.2.0', 'Negative', 'High',   76.50,
   'Major service outage affecting enterprise customer with 200 users. Notification system bug prevented alerting. Agent handled crisis communication well, committed to SLA credits and notification fix.',
   '["outage","enterprise","SLA credits","downtime","notification bug","database failover","critical"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call08, 'ServicePulse-AI-v1.2.0', 'Positive', 'Low',    87.00,
   'Customer called with feature request for custom dashboard widgets. Agent logged formal product feedback, enrolled customer in beta program. Positive engagement — no issue to resolve.',
   '["feature request","dashboard","custom widgets","beta program","product feedback"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call09, 'ServicePulse-AI-v1.2.0', 'Neutral',  'Low',    91.00,
   'GDPR data portability request handled correctly. Agent demonstrated strong knowledge of data retention policies and initiated Subject Access Request. Fully compliant response.',
   '["GDPR","data portability","SAR","privacy","data retention","right to erasure"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call10, 'ServicePulse-AI-v1.2.0', 'Positive', 'Low',    93.50,
   'Returning customer upgraded from 100 Mbps to 300 Mbps plan. Agent provided clear pricing comparison and processed upgrade efficiently. Good rapport with repeat customer.',
   '["upgrade","plan change","speed tier","returning customer","upsell"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call11, 'ServicePulse-AI-v1.2.0', 'Negative', 'Medium', 83.00,
   'Customer filed complaint about dismissive agent behavior in a previous call. Diana resolved the technical issue (sync conflict bug) and formally logged the complaint. Previous interaction needs QA review.',
   '["complaint","agent behavior","sync conflict","update","quality assurance","CMP-2891"]', 'Succeeded', NULL, GETUTCDATE()),

  (NEWID(), @Call12, 'ServicePulse-AI-v1.2.0', 'Neutral',  'Medium', 86.00,
   'Customer intended to cancel but was retained by downgrading to Lite plan. Agent identified usage pattern and matched appropriate tier. Successful retention — saved $29.99/month revenue.',
   '["cancellation","retention","downgrade","Lite plan","usage pattern","churn prevention"]', 'Succeeded', NULL, GETUTCDATE());

-- Analysis for Call 15 — Failed
INSERT INTO AnalysisResults (Id, CallId, AnalyzerVersion, Sentiment, RiskLevel, QualityScore, Summary, DetectedKeywordsJson, AnalysisStatus, ErrorMessage, CreatedAtUtc)
VALUES
  (NEWID(), @Call15, 'ServicePulse-AI-v1.2.0', 'Neutral', 'Low', 0.00,
   NULL, NULL, 'Failed',
   'Analysis pipeline timeout: transcript processing exceeded 120-second limit. Retry recommended.',
   GETUTCDATE());

-- ──────────────────────────────────────────────────────────────
-- 7. MANAGER REVIEWS (7 reviews)
-- ──────────────────────────────────────────────────────────────
INSERT INTO ManagerReviews (Id, ReviewerUserId, CallId, ReviewStatus, ManagerScore, CoachingNotes, OverrideReason, ReviewedAtUtc)
VALUES
  (NEWID(), @ManagerUserId, @Call01, 'Reviewed', 95.00,
   'Exemplary handling. Alice showed strong empathy, acknowledged the customer''s loyalty, and proactively offered compensation. Use this call as a training example.',
   NULL, GETUTCDATE()),

  (NEWID(), @ManagerUserId, @Call03, 'Reviewed', 78.00,
   'Alice handled a very difficult situation professionally. She acknowledged past failures, offered concrete next steps, and committed to follow-up. However, the underlying process failure (3 calls without resolution) needs to be flagged to the service delivery team. Coaching: continue building on de-escalation skills.',
   NULL, GETUTCDATE()),

  (NEWID(), @ManagerUserId, @Call05, 'Reviewed', 82.00,
   'Chandra followed refund policy correctly and was transparent. Good de-escalation by offering alternatives. Approved additional 1-month goodwill refund — escalation was appropriate.',
   NULL, GETUTCDATE()),

  (NEWID(), @ManagerUserId, @Call07, 'Reviewed', 85.00,
   'Diana handled a high-pressure enterprise outage call very professionally. She was transparent about the root cause, proactively identified the notification gap, and committed to concrete follow-up actions. Flag the notification propagation bug to engineering.',
   NULL, GETUTCDATE()),

  (NEWID(), @ManagerUserId, @Call09, 'Reviewed', 93.00,
   'Excellent GDPR handling. Chandra was thorough, accurate on retention periods, and proactively offered the privacy policy. No coaching needed — share with team as reference for privacy inquiries.',
   NULL, GETUTCDATE()),

  (NEWID(), @ManagerUserId, @Call11, 'Reviewed', 88.00,
   'Diana handled a sensitive situation skillfully — addressed both the complaint and the technical issue. Need to follow up on CMP-2891 and identify the previous agent for coaching. The sync issue resolution was technically accurate.',
   NULL, GETUTCDATE()),

  (NEWID(), @ManagerUserId, @Call12, 'Reviewed', 90.00,
   'Excellent retention save. Chandra listened to the customer''s reason, analyzed usage, and offered the right alternative without being pushy. Revenue preserved at $9.99/month vs total loss.',
   NULL, GETUTCDATE());

COMMIT TRANSACTION;
PRINT 'ServicePulse seed data inserted successfully!';
PRINT '  - 3 Application Users';
PRINT '  - 4 Agents';
PRINT '  - 8 Customers';
PRINT '  - 15 Calls';
PRINT '  - 15 Call Transcripts';
PRINT '  - 13 Analysis Results (12 Succeeded, 1 Failed)';
PRINT '  - 7 Manager Reviews';
GO

