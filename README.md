<h1>Es2al Project</h1>

<h2>About the Project</h2>
<p>
    Es2al is a dynamic Q&A platform where users can ask and answer questions in a structured, interactive environment.
    It uses a thread-based design where each new question automatically creates its own thread, keeping all related subquestions and answers organized in a clear hierarchy.
    It also includes advanced filtering, an infinite scroll browsing experience, a tag management system with admin control, and user engagement features.
</p>
<h2>Demo</h2>
<p>
    <a href="https://drive.google.com/file/d/1YGr87Nc2NFVPCv3-Rrl8ylNr0C4q2xMR/view?usp=sharing" target="_blank">Watch the Demo Video</a>
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
    <li><b>User Management</b>
        <ul>
            <li>User registration and authentication</li>
            <li>Role-based authorization (Admin, User)</li>
        </ul>
    </li>
    <li><b>User Engagement</b>
        <ul>
            <li>Follow/unfollow users for a personalized feed</li>
            <li>Notifications for user activities</li>
            <li>Inbox system for managing Q&A</li>
            <li>Like or dislike answers</li>
        </ul>
    </li>
    <li><b>Tag System</b>
        <ul>
            <li>Assign multiple tags to each question for better categorization</li>
            <li>Admin can create, edit, and delete tags</li>
            <li>Users can select preferred tags when editing their profile</li>
        </ul>
    </li>
    <li><b>Q&A Threading</b>
        <ul>
            <li>Each question belongs to a dedicated thread containing all its subquestions and answers</li>
            <li>Subquestions can be nested in a tree hierarchy for better discussion flow</li>
        </ul>
    </li>
    <li><b>Browsing Experience</b>
        <ul>
            <li>Filter questions by tags, date or search keyword </li>
            <li>Infinite scroll for seamless content loading</li>
        </ul>
    </li>
</ul>


<h2>Getting Started with Docker</h2>
<p>You can quickly run the Es2al application using Docker.</p>

<h3>Prerequisites</h3>
<ul>
    <li><a href="https://docs.docker.com/get-docker/" target="_blank">Docker</a> installed</li>
    <li><a href="https://docs.docker.com/compose/install/" target="_blank">Docker Compose</a> installed</li>
</ul>

<h3>Steps to Run</h3>
<ol>
    <li><b>Clone the repository</b>
        <pre><code>git clone https://github.com/ammar-gamal/Es2al.git
cd Es2al</code></pre>
    </li>
    <li><b>Build and start the containers</b>
        <pre><code>docker-compose up --build</code></pre>
    </li>
    <li><b>Access the application</b>
        <p>Open your browser and go to: <code>http://localhost:5000</code></p>
    </li>
    <li><b>Stopping the application</b>
        <pre><code>docker-compose down</code></pre>
    </li>
</ol>

<h2>ERD & Schema</h2>
<div style="align:center;">
    <h3>ERD</h3>
    <img src="/ERD.png" alt="ERD" style="width: 700px; height: 700px;">
    <h3>Schema</h3>
    <img src="/Schema.png" alt="Schema" style="width: 700px; height: 700px;">
</div>
