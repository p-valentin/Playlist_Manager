namespace Playlist_Manager;

public static class AppPages
{
    public const string IndexHtml = @"<!doctype html>
<html lang=""en""><head>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  <title>Playlist Manager</title>
  <style>
    :root {
      --bg: #f4f6fb;
      --card: #ffffff;
      --text: #111827;
      --muted: #6b7280;
      --accent: #2563eb;
      --accent-dark: #1e40af;
      --danger: #dc2626;
      --border: #e5e7eb;
    }
    * { box-sizing: border-box; }
    body {
      margin: 0;
      font-family: ""Segoe UI"", ""Inter"", Arial, sans-serif;
      background: var(--bg);
      color: var(--text);
    }
    .container {
      max-width: 1050px;
      margin: 32px auto;
      padding: 0 16px 40px;
    }
    header {
      display: flex;
      flex-wrap: wrap;
      gap: 12px;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 24px;
    }
    header h1 { margin: 0; font-size: 28px; }
    header p { margin: 0; color: var(--muted); }
    .card {
      background: var(--card);
      border-radius: 14px;
      padding: 16px;
      box-shadow: 0 10px 24px rgba(15, 23, 42, 0.08);
      margin-bottom: 16px;
    }
    .card h2 {
      margin: 0 0 12px;
      font-size: 18px;
    }
    .grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 12px;
    }
    label {
      display: block;
      font-size: 11px;
      letter-spacing: 0.08em;
      text-transform: uppercase;
      color: var(--muted);
      margin-bottom: 6px;
    }
    input, select {
      width: 100%;
      padding: 9px 12px;
      border-radius: 10px;
      border: 1px solid var(--border);
      font-size: 14px;
      background: #fff;
    }
    .actions {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      margin-top: 12px;
    }
    button {
      border: none;
      padding: 10px 14px;
      border-radius: 10px;
      cursor: pointer;
      font-weight: 600;
      background: var(--accent);
      color: #fff;
      transition: background 0.2s ease;
    }
    button:hover { background: var(--accent-dark); }
    button.secondary {
      background: #e5e7eb;
      color: #111827;
    }
    button.secondary:hover { background: #d1d5db; }
    button.danger {
      background: var(--danger);
    }
    .items {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    .item-row {
      display: flex;
      flex-wrap: wrap;
      align-items: center;
      justify-content: space-between;
      gap: 12px;
      padding: 12px;
      border-radius: 12px;
      border: 1px solid var(--border);
      background: #f9fafb;
    }
    .item-meta { display: flex; flex-direction: column; gap: 4px; }
    .badge {
      display: inline-block;
      padding: 3px 8px;
      background: #e0e7ff;
      color: #3730a3;
      border-radius: 999px;
      font-size: 12px;
    }
    .muted { color: var(--muted); font-size: 13px; }
    pre {
      background: #0f172a;
      color: #e2e8f0;
      padding: 12px;
      border-radius: 12px;
      overflow: auto;
      font-size: 12px;
      line-height: 1.5;
    }
  </style>
</head>
<body>
  <div class=""container"">
    <header>
      <div>
        <h1>Playlist Manager</h1>
        <p>Manage playlists, songs, and podcast episodes through the API.</p>
      </div>
    </header>

    <div class=""card"">
      <h2>Playlist</h2>
      <div class=""grid"">
        <div>
          <label for=""playlistName"">Playlist name</label>
          <input id=""playlistName"" value=""Demo"" />
        </div>
        <div>
          <label for=""quickSelect"">Quick select</label>
          <select id=""quickSelect"">
            <option value="""">Select...</option>
          </select>
        </div>
      </div>
      <div class=""actions"">
        <button id=""createBtn"">Create playlist</button>
        <button id=""loadBtn"" class=""secondary"">Load playlist</button>
        <button id=""deleteBtn"" class=""danger"">Delete playlist</button>
      </div>
      <p class=""muted"">New playlists are created empty. Add items below.</p>
    </div>

    <div class=""card"">
      <h2>Add item</h2>
      <div class=""grid"">
        <div>
          <label for=""itemType"">Type</label>
          <select id=""itemType"">
            <option value=""Song"">Song</option>
            <option value=""PodcastEpisode"">PodcastEpisode</option>
          </select>
        </div>
        <div>
          <label for=""itemTitle"">Title</label>
          <input id=""itemTitle"" placeholder=""Title"" />
        </div>
        <div>
          <label for=""itemDuration"">Duration (seconds)</label>
          <input id=""itemDuration"" placeholder=""Duration"" type=""number"" />
        </div>
      </div>
      <div class=""grid"" id=""songFields"">
        <div>
          <label for=""songArtist"">Artist</label>
          <input id=""songArtist"" placeholder=""Artist"" />
        </div>
        <div>
          <label for=""songAlbum"">Album</label>
          <input id=""songAlbum"" placeholder=""Album"" />
        </div>
      </div>
      <div class=""grid"" id=""podcastFields"" style=""display:none;"">
        <div>
          <label for=""podcastHost"">Host</label>
          <input id=""podcastHost"" placeholder=""Host"" />
        </div>
        <div>
          <label for=""podcastEpisode"">Episode number</label>
          <input id=""podcastEpisode"" placeholder=""Episode"" type=""number"" />
        </div>
      </div>
      <div class=""actions"">
        <button id=""addItemBtn"">Add item to playlist</button>
      </div>
    </div>

    <div class=""card"">
      <h2>Items in playlist</h2>
      <div id=""playlistItems"" class=""items"">
        <div class=""muted"">No playlist loaded.</div>
      </div>
    </div>

    <div class=""card"">
      <h2>API response</h2>
      <pre id=""output"">Ready.</pre>
    </div>
  </div>

  <script>
    const output = document.getElementById('output');
    const nameInput = document.getElementById('playlistName');
    const itemType = document.getElementById('itemType');
    const itemTitle = document.getElementById('itemTitle');
    const itemDuration = document.getElementById('itemDuration');
    const songFields = document.getElementById('songFields');
    const podcastFields = document.getElementById('podcastFields');
    const playlistItems = document.getElementById('playlistItems');
    const deleteBtn = document.getElementById('deleteBtn');
    let currentPlaylist = null;
    const quickSelect = document.getElementById('quickSelect');

    async function refreshQuickSelect() {
      try {
        const res = await fetch('/api/playlists');
        if (res.ok) {
           const list = await res.json();
           quickSelect.innerHTML = '<option value="""">Select...</option>';
           list.forEach(n => {
             const opt = document.createElement('option');
             opt.value = n;
             opt.textContent = n;
             quickSelect.appendChild(opt);
           });
        }
      } catch(e) { console.error(e); }
    }

    quickSelect.addEventListener('change', () => {
       if(quickSelect.value) {
           nameInput.value = quickSelect.value;
           document.getElementById('loadBtn').click();
       }
    });

    function updateFieldVisibility() {
      const isSong = itemType.value === 'Song';
      songFields.style.display = isSong ? 'grid' : 'none';
      podcastFields.style.display = isSong ? 'none' : 'grid';
    }

    itemType.addEventListener('change', updateFieldVisibility);
    updateFieldVisibility();

    function formatDuration(seconds) {
      const total = Number(seconds) || 0;
      const mins = Math.floor(total / 60);
      const secs = Math.round(total % 60);
      return mins > 0 ? `${mins}m ${secs}s` : `${secs}s`;
    }

    function renderPlaylist(playlist) {
      if (!playlist) {
        renderEmptyState();
        return;
      }
      if (!playlist.items || playlist.items.length === 0) {
        playlistItems.innerHTML = '<div class=""muted"">No items yet.</div>';
        return;
      }

      playlistItems.innerHTML = '';
      playlist.items.forEach(item => {
        const row = document.createElement('div');
        row.className = 'item-row';

        const meta = document.createElement('div');
        meta.className = 'item-meta';
        const detail = item.type === 'Song'
          ? `${item.artist || ''} • ${item.album || ''}`
          : `${item.host || ''} • Episode ${item.episodeNumber ?? ''}`;
        meta.innerHTML = `
          <div><span class=""badge"">${item.type}</span> <strong>${item.title}</strong></div>
          <div class=""muted"">${detail} • ${formatDuration(item.durationSeconds)}</div>
        `;

        const remove = document.createElement('button');
        remove.className = 'danger';
        remove.textContent = 'Delete';
        remove.addEventListener('click', async () => {
          const response = await fetch(`/api/playlists/${encodeURIComponent(nameInput.value)}/items/${item.id}`, {
            method: 'DELETE'
          });
          const data = await showResponse(response);
          if (response.ok && data && data.items) {
            currentPlaylist = data;
            renderPlaylist(currentPlaylist);
          }
        });

        row.appendChild(meta);
        row.appendChild(remove);
        playlistItems.appendChild(row);
      });
    }

    function renderEmptyState() {
      playlistItems.innerHTML = '<div class=""muted"">No playlist loaded.</div>';
    }

    async function showResponse(response) {
      const text = await response.text();
      try {
        const data = JSON.parse(text);
        output.textContent = JSON.stringify(data, null, 2);
        return data;
      } catch {
        output.textContent = text;
        return text;
      }
    }

    document.getElementById('createBtn').addEventListener('click', async () => {
      const payload = { name: nameInput.value, items: [] };
      const response = await fetch('/api/playlists', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });
      const data = await showResponse(response);
      if (response.ok && data && data.items) {
        currentPlaylist = data;
        renderPlaylist(currentPlaylist);
        refreshQuickSelect();
      }
    });

    document.getElementById('loadBtn').addEventListener('click', async () => {
      // Clear current items immediately
      currentPlaylist = null;
      renderEmptyState();
      
      const response = await fetch(`/api/playlists/${encodeURIComponent(nameInput.value)}`);
      
      if (response.status === 404) {
        alert(""Playlist not found"");
        output.textContent = ""Playlist not found"";
        return;
      }

      const data = await showResponse(response);
      if (response.ok && data && data.items) {
        currentPlaylist = data;
        renderPlaylist(currentPlaylist);
      }
    });

    deleteBtn.addEventListener('click', async () => {
      const response = await fetch(`/api/playlists/${encodeURIComponent(nameInput.value)}`, {
        method: 'DELETE'
      });
      await showResponse(response);
      if (response.ok) {
        currentPlaylist = null;
        renderEmptyState();
        refreshQuickSelect();
      }
    });

    document.getElementById('addItemBtn').addEventListener('click', async () => {
      const payload = {
        type: itemType.value,
        title: itemTitle.value,
        durationSeconds: Number(itemDuration.value || 0)
      };

      if (itemType.value === 'Song') {
        payload.artist = document.getElementById('songArtist').value;
        payload.album = document.getElementById('songAlbum').value;
      } else {
        payload.host = document.getElementById('podcastHost').value;
        payload.episodeNumber = Number(document.getElementById('podcastEpisode').value || 0);
      }

      const response = await fetch(`/api/playlists/${encodeURIComponent(nameInput.value)}/items`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });

      const data = await showResponse(response);
      if (response.ok && data && data.items) {
        currentPlaylist = data;
        renderPlaylist(currentPlaylist);
      }
    });

    refreshQuickSelect();
  </script>
</body>
</html>";
}