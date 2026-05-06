using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;
using System.Text.Json;

namespace SchoolManagmentSystem.ServicesAi.EmploymentAI
{
    public class AIScoringService
    {
        private readonly string _apikey = "";

        public AIScoringService(IConfiguration configuration)
        {
            _apikey = configuration["ApiSettings:ApiKey2"];
        }

        public async Task<(int Score, string Summary)> GetSuitabilityScore(string cvText, string role, string? quizanswers, string? interviewNotes)
        {
            try
            {
                var googleAI = new GoogleAI(_apikey);
                var model = googleAI.GenerativeModel(Model.Gemini3Flash);

                string prompt = $@"Analyze the following CV for the position of {role}.

ROLE-SPECIFIC INSTRUCTIONS:
- If the role is 'Teacher', ONLY evaluate against the Teacher quiz.
- If the role is 'Cleaner', ONLY evaluate against the Cleaner quiz.
- You MUST completely ignore the quiz for the other role as if it does not exist.
- Do NOT compare answers across quizzes.
- Do NOT assume mismatch between quizzes.

CV EVALUATION:
- Teacher: Look for education degrees, classroom experience, and special needs experience (especially autism).
- Cleaner: Look for physical reliability, cleaning experience, and safety awareness.

QUIZ USAGE RULE:
Only use quiz answers if and only if the string ""quiz answers:"" is followed by actual user-provided answers.

If the value is:
- empty
- null
- whitespace
- ""NONE""
- ""N/A""

Then:
- Ignore the quiz completely
- Do NOT mention missing or invalid quiz answers

CRITICAL:
- Evaluate quiz answers ONLY against the correct role’s quiz
- If answers do not align with that role’s quiz, treat them as irrelevant or incorrect
- Do NOT try to match them to another role’s quiz

INTERVIEW NOTES RULE:
Only use interview notes if and only if the string ""interview notes:"" is followed by actual content.

If the value is:
- empty
- null
- whitespace
- ""NONE""
- ""N/A""

Then:
- You MUST completely ignore the interview notes
- You MUST NOT reference, analyze, or infer anything from them
- You MUST NOT mention that interview notes were missing

If valid interview notes are present:
- Analyze them for communication skills, attitude, professionalism, and role suitability
- Use them as supporting evidence, not the sole deciding factor

OUTPUT:
Provide a clear evaluation of:
1. CV strength
2. Quiz performance (if valid)
3. Interview Notes
4. Final suitability decision

Quiz Questions (Teacher):
1. When designing a lesson plan, which of the ""Three Pillars of Backward Design"" should be determined first?
A) Selecting the learning activities and textbooks  
B) Identifying the desired results (learning objectives)  
C) Planning the classroom layout  
D) Determining the homework schedule  

2. A student becomes suddenly withdrawn and grades drop. What is your initial response?
A) Give them a failing grade to motivate them  
B) Mention it to the class as an example  
C) Request a private check-in with the student to offer support  
D) Immediately call a board meeting  

3. Which strategy best promotes ""Active Learning"" in a classroom?
A) Reading the textbook aloud  
B) Students engaging in a 'Think-Pair-Share' discussion  
C) Handing out pre-written notes for memorization  
D) Watching a movie without follow-up  

4. According to privacy guidelines (FERPA), it is acceptable to:
A) Post student grades by name on the door  
B) Discuss behavior with another student's parent  
C) Share records with officials who have legitimate educational interest  
D) Give home addresses to local businesses  

5. Describe a time you had to adapt a lesson on the fly because students weren't grasping a concept. What did you change?

6. How do you handle a situation where two students are involved in a heated verbal disagreement during your instruction?

7. How do you ensure that students with varying physical or cognitive abilities feel fully included in your daily classroom activities?

Quiz Questions (Cleaner):
1. What does the 'Contact Time' on a disinfectant label refer to?
    A) The time it takes to pour the liquid into a bucket.
    B) How long the surface must remain visibly wet with the disinfectant to kill germs.
    C) The expiration date of the cleaning solution.
    D) The amount of time the cleaner is allowed to be in the building.

2. If you accidentally mix bleach with an ammonia-based cleaner, what is the result?
    A) A more powerful cleaning solution for tough stains.
    B) A pleasant-smelling soap.
    C) The creation of toxic chloramine gas, which is highly dangerous.
    D) Nothing; the two chemicals neutralize each other and become water.

3. You are cleaning a glass entrance door and notice a small crack in the pane. What is the correct procedure?
    A) Tape it over and continue cleaning.
    B) Ignore it, as it is a maintenance issue, not a cleaning issue.
    C) Clean the other side only to avoid breaking it further.
    D) Document the damage and report it to your supervisor immediately for safety reasons.

4. Which of the following is an example of 'Cross-Contamination' in a cleaning context?
    A) Using a different colored cloth for the bathroom and the breakroom.
    B) Using the same mop head to clean a restroom floor and then a cafeteria floor.
    C) Wearing gloves while handling trash.
    D) Refilling a soap dispenser before it is completely empty.

5. Efficiency: You have two hours to clean five offices and a large conference room. How do you prioritize your tasks to ensure the highest quality of work?

6. Safety Awareness: Describe the steps you take to protect yourself and others when cleaning up a spill involving broken glass.

7. Reliability: Our facility requires high standards of cleanliness for health reasons. Describe your personal process for double-checking your work before leaving a room.


CV Text: {cvText}
quiz answers: {quizanswers}
interview notes: {interviewNotes}

Return ONLY a Json object with two fields: 'score' (an integer from 1-10) and 'summary' (a brief explanation) No markdown, no ```json tags.";

                var response = await model.GenerateContent(prompt);
                var jsonResponse = response.Text;

                Console.WriteLine("RAW AI RESPONSE: " + jsonResponse);

                if (jsonResponse.StartsWith("```"))
                {
                    jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "").Trim();
                }
                
                var result = JsonSerializer.Deserialize<AIResult>(jsonResponse);

                return (result?.score ?? 0, result?.summary ?? "No summary provided.");
            }
            catch (Exception ex)
            {
                return (0, "AI scoring failed: " + ex.Message);
            }
        }


    }

    public class AIResult
    {
        public int score { get; set; }
        public string summary { get; set; }
    }
}
