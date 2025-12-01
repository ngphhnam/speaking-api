-- Sample Data for Speaking Practice API
-- This script inserts realistic data for testing and development

-- ============================================
-- 1. USERS (ApplicationUser)
-- ============================================
-- Note: Users table uses Identity, so we need to insert through Identity or use proper password hashing
-- For testing, you may need to register users through the API or use Identity's UserManager

-- Sample user IDs (replace with actual user IDs from your system)
-- User 1: John Smith (Intermediate, Target: 7.0)
-- User 2: Sarah Johnson (Advanced, Target: 8.0)
-- User 3: Michael Chen (Beginner, Target: 6.5)
-- User 4: Emma Wilson (Intermediate, Target: 7.5)

-- ============================================
-- 2. TOPICS (IELTS Speaking Topics)
-- ============================================
INSERT INTO topics (id, title, slug, description, part_number, difficulty_level, topic_category, keywords, usage_count, avg_user_rating, is_active, created_at, updated_at) VALUES
-- Part 1 Topics (Personal Questions)
('a1b2c3d4-e5f6-4789-a012-345678901234', 'Hometown', 'hometown', 'Questions about your hometown, where you live, and your local area', 1, 'beginner', 'personal', ARRAY['hometown', 'city', 'place', 'location'], 45, 4.2, true, NOW(), NOW()),
('b2c3d4e5-f6a7-4890-b123-456789012345', 'Work', 'work', 'Questions about your job, career, and professional life', 1, 'intermediate', 'professional', ARRAY['work', 'job', 'career', 'profession'], 38, 4.5, true, NOW(), NOW()),
('c3d4e5f6-a7b8-4901-c234-567890123456', 'Studies', 'studies', 'Questions about your education, school, and learning', 1, 'beginner', 'education', ARRAY['study', 'education', 'school', 'university'], 52, 4.3, true, NOW(), NOW()),
('d4e5f6a7-b8c9-4012-d345-678901234567', 'Hobbies', 'hobbies', 'Questions about your interests, hobbies, and free time activities', 1, 'beginner', 'lifestyle', ARRAY['hobby', 'interest', 'leisure', 'activity'], 41, 4.4, true, NOW(), NOW()),

-- Part 2 Topics (Long Turn - Cue Cards)
('e5f6a7b8-c9d0-4123-e456-789012345678', 'Describe a Person', 'describe-a-person', 'Describe someone who influenced you, a family member, or a famous person', 2, 'intermediate', 'descriptive', ARRAY['person', 'describe', 'character', 'influence'], 67, 4.6, true, NOW(), NOW()),
('f6a7b8c9-d0e1-4234-f567-890123456789', 'Describe a Place', 'describe-a-place', 'Describe a place you visited, want to visit, or a memorable location', 2, 'intermediate', 'descriptive', ARRAY['place', 'location', 'travel', 'visit'], 73, 4.7, true, NOW(), NOW()),
('a7b8c9d0-e1f2-4345-a678-901234567890', 'Describe an Event', 'describe-an-event', 'Describe a memorable event, celebration, or experience', 2, 'intermediate', 'narrative', ARRAY['event', 'celebration', 'experience', 'occasion'], 58, 4.5, true, NOW(), NOW()),
('b8c9d0e1-f2a3-4456-b789-012345678901', 'Describe an Object', 'describe-an-object', 'Describe something you own, want to buy, or find useful', 2, 'intermediate', 'descriptive', ARRAY['object', 'item', 'thing', 'possession'], 49, 4.4, true, NOW(), NOW()),

-- Part 3 Topics (Discussion - Abstract)
('c9d0e1f2-a3b4-4567-c890-123456789012', 'Education System', 'education-system', 'Discussion about education, learning methods, and academic systems', 3, 'advanced', 'academic', ARRAY['education', 'learning', 'academic', 'system'], 35, 4.8, true, NOW(), NOW()),
('d0e1f2a3-b4c5-4678-d901-234567890123', 'Technology Impact', 'technology-impact', 'Discussion about technology, its effects on society and daily life', 3, 'advanced', 'society', ARRAY['technology', 'digital', 'innovation', 'impact'], 42, 4.7, true, NOW(), NOW()),
('e1f2a3b4-c5d6-4789-e012-345678901234', 'Environment', 'environment', 'Discussion about environmental issues, climate change, and sustainability', 3, 'advanced', 'society', ARRAY['environment', 'climate', 'pollution', 'sustainability'], 39, 4.6, true, NOW(), NOW()),
('f2a3b4c5-d6e7-4890-f123-456789012345', 'Work-Life Balance', 'work-life-balance', 'Discussion about work, career, and balancing professional and personal life', 3, 'advanced', 'lifestyle', ARRAY['work', 'balance', 'career', 'lifestyle'], 31, 4.5, true, NOW(), NOW());

-- ============================================
-- 3. QUESTIONS (IELTS Speaking Questions)
-- ============================================
INSERT INTO questions (id, topic_id, question_text, question_type, suggested_structure, sample_answers, key_vocabulary, estimated_band_requirement, time_limit_seconds, attempts_count, avg_score, is_active, created_at, updated_at) VALUES
-- Part 1 Questions
('00010001-0000-0000-0000-000000000001', 'a1b2c3d4-e5f6-4789-a012-345678901234', 'Where are you from?', 'personal', '"Introduction - State your hometown\nDetails - Describe location and size\nPersonal connection - How long you''ve lived there"'::jsonb, ARRAY[]::text[], ARRAY['hometown', 'located', 'reside', 'originally'], 5.0, 60, 23, 6.2, true, NOW(), NOW()),
('00010001-0000-0000-0000-000000000002', 'a1b2c3d4-e5f6-4789-a012-345678901234', 'What do you like most about your hometown?', 'personal', '"Introduction - State your favorite aspect\nMain point - Specific feature with example\nConclusion - Why it matters to you"'::jsonb, ARRAY[]::text[], ARRAY['appreciate', 'favorite', 'attraction', 'characteristic'], 5.5, 60, 18, 6.5, true, NOW(), NOW()),
('00010001-0000-0000-0000-000000000003', 'b2c3d4e5-f6a7-4890-b123-456789012345', 'What is your job?', 'personal', '"Introduction - State your profession\nDetails - Describe your role and responsibilities\nPersonal opinion - How you feel about it"'::jsonb, ARRAY[]::text[], ARRAY['profession', 'occupation', 'role', 'responsibilities'], 5.5, 60, 31, 6.8, true, NOW(), NOW()),
('00010001-0000-0000-0000-000000000004', 'b2c3d4e5-f6a7-4890-b123-456789012345', 'Do you enjoy your work?', 'personal', '"Introduction - State your opinion\nMain point 1 - What you like\nMain point 2 - Challenges or dislikes\nConclusion - Overall feeling"'::jsonb, ARRAY[]::text[], ARRAY['enjoy', 'satisfaction', 'challenging', 'rewarding'], 6.0, 60, 27, 7.0, true, NOW(), NOW()),

-- Part 2 Questions (Cue Cards)
('00020001-0000-0000-0000-000000000001', 'e5f6a7b8-c9d0-4123-e456-789012345678', 'Describe a person who has influenced you. You should say:\n- Who this person is\n- How you know them\n- What they have done to influence you\n- And explain why this person is important to you', 'cue_card', '"Introduction - Name the person\nWho they are - Relationship and background\nHow you know them - Context of your relationship\nTheir influence - Specific examples of impact\nWhy important - Personal significance\nConclusion - Summary"'::jsonb, ARRAY[]::text[], ARRAY['influence', 'inspire', 'mentor', 'role model', 'impact'], 6.5, 120, 15, 7.2, true, NOW(), NOW()),
('00020001-0000-0000-0000-000000000002', 'f6a7b8c9-d0e1-4234-f567-890123456789', 'Describe a place you would like to visit. You should say:\n- Where it is\n- What you know about it\n- What you would like to do there\n- And explain why you want to visit this place', 'cue_card', '"Introduction - Name the place\nLocation - Where it is geographically\nWhat you know - Facts and information\nActivities - What you''d like to do\nWhy visit - Reasons and motivation\nConclusion - Summary"'::jsonb, ARRAY[]::text[], ARRAY['destination', 'landmark', 'attraction', 'itinerary', 'explore'], 7.0, 120, 12, 7.5, true, NOW(), NOW()),
('00020001-0000-0000-0000-000000000003', 'a7b8c9d0-e1f2-4345-a678-901234567890', 'Describe a memorable event in your life. You should say:\n- When it happened\n- Where it took place\n- What happened\n- And explain why it was memorable', 'cue_card', '"Introduction - Name the event\nWhen - Time and context\nWhere - Location details\nWhat happened - Sequence of events\nWhy memorable - Significance and impact\nConclusion - Reflection"'::jsonb, ARRAY[]::text[], ARRAY['memorable', 'significant', 'milestone', 'celebration', 'experience'], 6.5, 120, 19, 7.1, true, NOW(), NOW()),

-- Part 3 Questions (Discussion)
('00030001-0000-0000-0000-000000000001', 'c9d0e1f2-a3b4-4567-c890-123456789012', 'What do you think are the main advantages and disadvantages of the education system in your country?', 'discussion', '"Introduction - State your view\nMain point 1 - Advantage with example\nMain point 2 - Disadvantage with example\nComparison - How it could be improved\nConclusion - Summary"'::jsonb, ARRAY[]::text[], ARRAY['advantage', 'disadvantage', 'curriculum', 'pedagogy', 'assessment'], 7.5, 120, 8, 7.8, true, NOW(), NOW()),
('00030001-0000-0000-0000-000000000002', 'd0e1f2a3-b4c5-4678-d901-234567890123', 'How has technology changed the way people communicate?', 'discussion', '"Introduction - Acknowledge the change\nMain point 1 - Positive changes with examples\nMain point 2 - Negative aspects\nFuture implications - What might happen\nConclusion - Balanced view"'::jsonb, ARRAY[]::text[], ARRAY['technology', 'communication', 'digital', 'impact', 'transformation'], 8.0, 120, 6, 8.2, true, NOW(), NOW()),
('00030001-0000-0000-0000-000000000003', 'e1f2a3b4-c5d6-4789-e012-345678901234', 'What are the main environmental problems facing the world today?', 'discussion', '"Introduction - State the issue\nProblem 1 - Climate change with details\nProblem 2 - Pollution with examples\nProblem 3 - Resource depletion\nSolutions - What can be done\nConclusion - Urgency and importance"'::jsonb, ARRAY[]::text[], ARRAY['environmental', 'climate change', 'pollution', 'sustainability', 'conservation'], 8.5, 120, 5, 8.5, true, NOW(), NOW());

-- ============================================
-- 4. VOCABULARY (IELTS Vocabulary Words)
-- ============================================
INSERT INTO vocabulary (id, word, phonetic, part_of_speech, definition_en, definition_vi, ielts_band_level, topic_categories, example_sentences, synonyms, antonyms, collocations, usage_frequency, created_at, updated_at) VALUES
('0000a001-0000-0000-0000-000000000001', 'significant', '/sɪɡˈnɪfɪkənt/', 'adjective', 'Important or noticeable', 'Quan trọng, đáng kể', 6.5, ARRAY['academic', 'general'], ARRAY['The changes have had a significant impact on the environment.', 'This is a significant achievement for the team.'], ARRAY['important', 'substantial', 'notable'], ARRAY['insignificant', 'minor', 'trivial'], ARRAY['significant impact', 'significant change', 'significant difference'], 45, NOW(), NOW()),
('0000a002-0000-0000-0000-000000000002', 'sustainable', '/səˈsteɪnəbəl/', 'adjective', 'Able to be maintained at a certain rate or level', 'Bền vững', 7.0, ARRAY['environment', 'society'], ARRAY['We need to find sustainable solutions to environmental problems.', 'Sustainable development is crucial for future generations.'], ARRAY['maintainable', 'viable', 'enduring'], ARRAY['unsustainable', 'temporary'], ARRAY['sustainable development', 'sustainable energy', 'sustainable practice'], 38, NOW(), NOW()),
('0000a003-0000-0000-0000-000000000003', 'influence', '/ˈɪnfluəns/', 'noun/verb', 'The capacity to have an effect on someone or something', 'Ảnh hưởng', 6.0, ARRAY['general', 'social'], ARRAY['Her teacher had a great influence on her career choice.', 'Social media can influence public opinion.'], ARRAY['impact', 'effect', 'sway'], ARRAY['uninfluenced'], ARRAY['have influence', 'under the influence', 'influence on'], 52, NOW(), NOW()),
('0000a004-0000-0000-0000-000000000004', 'innovative', '/ˈɪnəveɪtɪv/', 'adjective', 'Featuring new methods; advanced and original', 'Sáng tạo, đổi mới', 7.5, ARRAY['technology', 'business'], ARRAY['The company is known for its innovative approach to problem-solving.', 'Innovative technologies are transforming healthcare.'], ARRAY['creative', 'original', 'groundbreaking'], ARRAY['conventional', 'traditional'], ARRAY['innovative solution', 'innovative technology', 'innovative approach'], 29, NOW(), NOW()),
('0000a005-0000-0000-0000-000000000005', 'comprehensive', '/ˌkɒmprɪˈhensɪv/', 'adjective', 'Complete and including everything', 'Toàn diện, bao quát', 7.0, ARRAY['academic', 'general'], ARRAY['The report provides a comprehensive analysis of the situation.', 'We need a comprehensive approach to solve this problem.'], ARRAY['complete', 'thorough', 'extensive'], ARRAY['incomplete', 'partial'], ARRAY['comprehensive study', 'comprehensive review', 'comprehensive coverage'], 33, NOW(), NOW()),
('0000a006-0000-0000-0000-000000000006', 'controversial', '/ˌkɒntrəˈvɜːʃəl/', 'adjective', 'Causing disagreement or discussion', 'Gây tranh cãi', 7.0, ARRAY['society', 'politics'], ARRAY['This is a controversial topic that divides public opinion.', 'The decision was controversial among experts.'], ARRAY['disputed', 'debated', 'contentious'], ARRAY['uncontroversial', 'agreed'], ARRAY['controversial issue', 'controversial decision', 'controversial topic'], 27, NOW(), NOW()),
('0000a007-0000-0000-0000-000000000007', 'profound', '/prəˈfaʊnd/', 'adjective', 'Very great or intense', 'Sâu sắc, thâm thúy', 8.0, ARRAY['academic', 'philosophy'], ARRAY['The book had a profound effect on my thinking.', 'There are profound differences between the two approaches.'], ARRAY['deep', 'intense', 'significant'], ARRAY['superficial', 'shallow'], ARRAY['profound impact', 'profound change', 'profound understanding'], 19, NOW(), NOW()),
('0000a008-0000-0000-0000-000000000008', 'diverse', '/daɪˈvɜːs/', 'adjective', 'Showing a great deal of variety', 'Đa dạng', 6.5, ARRAY['society', 'culture'], ARRAY['The city has a diverse population from many countries.', 'We need diverse perspectives to solve complex problems.'], ARRAY['varied', 'mixed', 'heterogeneous'], ARRAY['homogeneous', 'uniform'], ARRAY['diverse range', 'diverse population', 'diverse culture'], 41, NOW(), NOW());

-- ============================================
-- 5. ACHIEVEMENTS (Gamification)
-- ============================================
INSERT INTO achievements (id, title, description, achievement_type, requirement_criteria, points, badge_icon_url, is_active, created_at) VALUES
('0000b001-0000-0000-0000-000000000001', 'First Steps', 'Complete your first practice session', 'practice_milestone', '{"sessions_completed": 1}', 10, '/badges/first-steps.png', true, NOW()),
('0000b002-0000-0000-0000-000000000002', 'Week Warrior', 'Practice for 7 consecutive days', 'practice_streak', '{"consecutive_days": 7}', 50, '/badges/week-warrior.png', true, NOW()),
('0000b003-0000-0000-0000-000000000003', 'Band 7 Achiever', 'Score 7.0 or higher on any practice', 'score_milestone', '{"min_score": 7.0}', 100, '/badges/band-7.png', true, NOW()),
('0000b004-0000-0000-0000-000000000004', 'Vocabulary Master', 'Master 50 vocabulary words', 'vocabulary_milestone', '{"words_mastered": 50}', 75, '/badges/vocab-master.png', true, NOW()),
('0000b005-0000-0000-0000-000000000005', 'Practice Champion', 'Complete 50 practice sessions', 'practice_milestone', '{"sessions_completed": 50}', 150, '/badges/champion.png', true, NOW()),
('0000b006-0000-0000-0000-000000000006', 'Perfect Week', 'Score above 7.0 in all sessions for a week', 'score_milestone', '{"min_score": 7.0, "period": "week", "all_sessions": true}', 200, '/badges/perfect-week.png', true, NOW());

-- ============================================
-- NOTES FOR USERS AND RELATED DATA
-- ============================================
-- The following tables require actual user IDs from your Identity system:
-- - practice_sessions (requires user_id)
-- - recordings (requires user_id, session_id)
-- - analysis_results (requires user_id, recording_id)
-- - user_progress (requires user_id)
-- - user_vocabulary (requires user_id, vocabulary_id)
-- - user_achievements (requires user_id, achievement_id)
-- - user_drafts (requires user_id, question_id)

-- To insert data for these tables, you'll need to:
-- 1. Register users through your API or use Identity UserManager
-- 2. Get the actual user IDs (GUIDs)
-- 3. Replace the placeholder user IDs in the queries below

-- Example queries (replace USER_ID_PLACEHOLDER with actual user IDs):

/*
-- PRACTICE SESSIONS
INSERT INTO practice_sessions (id, user_id, session_type, part_number, topic_id, questions_attempted, total_duration_seconds, started_at, completed_at, status, overall_band_score, fluency_score, vocabulary_score, grammar_score, pronunciation_score, device_info, created_at, updated_at) VALUES
('s001-0000-0000-0000-000000000001', 'USER_ID_PLACEHOLDER', 'practice', 2, 'e5f6a7b8-c9d0-4123-e456-789012345678', 1, 180, NOW() - INTERVAL '2 days', NOW() - INTERVAL '2 days', 'completed', 7.0, 7.2, 6.8, 7.1, 6.9, '{"device": "Chrome", "os": "Windows"}', NOW() - INTERVAL '2 days', NOW() - INTERVAL '2 days');

-- RECORDINGS
INSERT INTO recordings (id, session_id, user_id, question_id, audio_url, audio_format, file_size_bytes, duration_seconds, transcription_text, transcription_confidence, transcription_language, processing_status, recorded_at, processed_at, created_at) VALUES
('r001-0000-0000-0000-000000000001', 's001-0000-0000-0000-000000000001', 'USER_ID_PLACEHOLDER', 'q002-0000-0000-0000-000000000001', 'https://storage.example.com/audio/recording1.mp3', 'mp3', 2456789, 125.5, 'I would like to describe my high school teacher, Mrs. Johnson, who has had a profound influence on my life...', 0.95, 'en', 'completed', NOW() - INTERVAL '2 days', NOW() - INTERVAL '2 days', NOW() - INTERVAL '2 days');

-- ANALYSIS RESULTS
INSERT INTO analysis_results (id, recording_id, user_id, overall_band_score, fluency_score, vocabulary_score, grammar_score, pronunciation_score, metrics, feedback_summary, strengths, improvements, grammar_issues, pronunciation_issues, vocabulary_suggestions, analyzed_at, created_at) VALUES
('ar001-0000-0000-0000-000000000001', 'r001-0000-0000-0000-000000000001', 'USER_ID_PLACEHOLDER', 7.0, 7.2, 6.8, 7.1, 6.9, '{"words_per_minute": 145, "pause_frequency": 2.3}', 'Good overall performance with clear structure and relevant vocabulary.', ARRAY['Clear pronunciation', 'Good use of linking words', 'Well-organized response'], ARRAY['Use more advanced vocabulary', 'Reduce hesitation', 'Work on intonation'], ARRAY['Minor article errors'], ARRAY['Some word stress issues'], ARRAY['Consider using "profound" instead of "big"', 'Use "influential" more naturally'], NOW() - INTERVAL '2 days', NOW() - INTERVAL '2 days');

-- USER PROGRESS
INSERT INTO user_progress (id, user_id, period_type, period_start, period_end, total_sessions, total_recordings, total_practice_minutes, avg_overall_score, avg_fluency_score, avg_vocabulary_score, avg_grammar_score, avg_pronunciation_score, score_improvement, consistency_score, topics_practiced, weakest_areas, strongest_areas, created_at, updated_at) VALUES
('up001-0000-0000-0000-000000000001', 'USER_ID_PLACEHOLDER', 'weekly', CURRENT_DATE - INTERVAL '7 days', CURRENT_DATE, 5, 5, 45, 7.0, 7.1, 6.9, 7.0, 6.8, 0.5, 85.0, '{"topics": ["describe-a-person", "describe-a-place"]}', ARRAY['Pronunciation', 'Vocabulary range'], ARRAY['Fluency', 'Grammar'], NOW() - INTERVAL '3 days', NOW());

-- USER VOCABULARY
INSERT INTO user_vocabulary (id, user_id, vocabulary_id, learning_status, next_review_at, review_count, success_count, personal_notes, example_usage, first_encountered_at, last_reviewed_at, mastered_at, created_at) VALUES
('uv001-0000-0000-0000-000000000001', 'USER_ID_PLACEHOLDER', 'v001-0000-0000-0000-000000000001', 'learning', NOW() + INTERVAL '2 days', 3, 2, 'Important word for academic writing', 'The changes had a significant impact on the environment.', NOW() - INTERVAL '5 days', NOW() - INTERVAL '1 day', NULL, NOW() - INTERVAL '5 days');

-- USER ACHIEVEMENTS
INSERT INTO user_achievements (id, user_id, achievement_id, progress, is_completed, earned_at, created_at) VALUES
('ua001-0000-0000-0000-000000000001', 'USER_ID_PLACEHOLDER', 'a001-0000-0000-0000-000000000001', '{"sessions_completed": 1}', true, NOW() - INTERVAL '10 days', NOW() - INTERVAL '10 days'),
('ua002-0000-0000-0000-000000000002', 'USER_ID_PLACEHOLDER', 'a003-0000-0000-0000-000000000003', '{"max_score": 7.2}', true, NOW() - INTERVAL '2 days', NOW() - INTERVAL '2 days');

-- USER DRAFTS
INSERT INTO user_drafts (id, user_id, question_id, draft_content, outline_structure, created_at, updated_at) VALUES
('ud001-0000-0000-0000-000000000001', 'USER_ID_PLACEHOLDER', 'q002-0000-0000-0000-000000000001', 'I would like to describe my teacher Mrs. Johnson...', '{"introduction": "State person", "body": ["Who they are", "How I know them", "Their influence"], "conclusion": "Why important"}', NOW() - INTERVAL '1 day', NOW());
*/

-- ============================================
-- USAGE INSTRUCTIONS
-- ============================================
-- 1. Run this script in your PostgreSQL database
-- 2. For user-related data, first create users through your API
-- 3. Replace USER_ID_PLACEHOLDER with actual user GUIDs
-- 4. Adjust timestamps if needed for testing different scenarios
-- 5. You can modify the data to match your specific testing needs

