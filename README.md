<h1>Es2al Project</h1>

<h2>About the Project</h2>
<p>
    Es2al is a dynamic Q&A platform where users can ask and answer questions in a structured, interactive environment.
    It uses a thread-based design where each new question automatically creates its own thread, and all related subquestions are kept within that same thread. This ensures discussions remain organized and supports hierarchical subquestions and answers.
    It also includes advanced filtering, an infinite scroll browsing experience, a tag management system with admin control, and user engagement features.
</p>

<h2>Technologies Used</h2>
<ul>
    <li><b>Backend:</b> C#, ASP.NET MVC Framework, Entity Framework Core</li>
    <li><b>Frontend:</b> HTML, CSS, Bootstrap, JavaScript</li>
    <li><b>Database:</b> SQL Server</li>
    <li><b>Testing:</b> xUnit, Moq, FluentAssertions</li>
</ul>

<h2>Features</h2>
<ul>
    <li>User registration and authentication.</li>
    <li>Role-based authorization (Admin, User).</li>
    <li>Follow/unfollow users for a personalized feed.</li>
    <li>Notifications for user activities.</li>
    <li>Inbox system for managing Q&A.</li>
    <li>User can like or dislike answers.</li>
    <li>Tag system for categorizing questions.
     <ul>
        <li>Each question can be assigned multiple tags for better categorization</li>
        <li>Admin can create, edit, and delete tags.</li>
        <li>Users can select preferred tags when editing their profile.</li>
     </ul>
    </li>
    <li>Question-Answer System
        <ul>
            <li>Each question belongs to a thread, allowing for multiple subquestions and answers.</li>
            <li>Subquestions can be nested to form a tree hierarchy for better discussion flow.</li>
        </ul>
    </li>
    <li>Filtering and Infinite Scroll
        <ul>
            <li>Easily filter questions based on tags, date, or other criteria.</li>
            <li>Enjoy a seamless browsing experience with infinite scroll to load more content dynamically.</li>
        </ul>
    </li>
</ul>

<h2>Demo</h2>
<p>
    <a href="https://drive.google.com/file/d/1YGr87Nc2NFVPCv3-Rrl8ylNr0C4q2xMR/view?usp=sharing" target="_blank">Watch the Demo Video</a>
</p>

<h2>ERD & Schema</h2>
<div style="align:center;">
    <h3>ERD</h3>
    <img src="/ERD.png" alt="ERD" style="width: 700px; height: 700px;">
    <h3>Schema</h3>
    <img src="/Schema.png" alt="Schema" style="width: 700px; height: 700px;">
</div>
