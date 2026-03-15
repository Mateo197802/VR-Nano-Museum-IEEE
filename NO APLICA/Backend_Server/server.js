require('dotenv').config();
const express = require('express');
const cors = require('cors');
const db = require('./database');

const app = express();
const PORT = process.env.PORT || 3000;

// Middlewares
app.use(cors());
app.use(express.json());

// --- ROUTES FOR SCIENTISTS ---

app.get('/api/scientists', (req, res) => {
    db.all("SELECT * FROM scientists ORDER BY position_index ASC", [], (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows);
    });
});

app.post('/api/scientists', (req, res) => {
    const { name, description, image_url, position_index } = req.body;
    db.run(
        `INSERT INTO scientists (name, description, image_url, position_index) VALUES (?, ?, ?, ?)`,
        [name, description, image_url, position_index || 0],
        function(err) {
            if (err) return res.status(500).json({ error: err.message });
            res.status(201).json({ id: this.lastID });
        }
    );
});

app.put('/api/scientists/:id', (req, res) => {
    const { name, description, image_url, position_index } = req.body;
    db.run(
        `UPDATE scientists SET name = ?, description = ?, image_url = ?, position_index = ? WHERE id = ?`,
        [name, description, image_url, position_index, req.params.id],
        function(err) {
            if (err) return res.status(500).json({ error: err.message });
            res.json({ updated: this.changes });
        }
    );
});

app.delete('/api/scientists/:id', (req, res) => {
    db.run(`DELETE FROM scientists WHERE id = ?`, req.params.id, function(err) {
        if (err) return res.status(500).json({ error: err.message });
        res.json({ deleted: this.changes });
    });
});

// --- ROUTES FOR NANOTOPICS ---

app.get('/api/nanotopics', (req, res) => {
    db.all("SELECT * FROM nanotopics ORDER BY position_index ASC", [], (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        res.json(rows);
    });
});

app.post('/api/nanotopics', (req, res) => {
    const { title, content, video_url, position_index } = req.body;
    db.run(
        `INSERT INTO nanotopics (title, content, video_url, position_index) VALUES (?, ?, ?, ?)`,
        [title, content, video_url, position_index || 0],
        function(err) {
            if (err) return res.status(500).json({ error: err.message });
            res.status(201).json({ id: this.lastID });
        }
    );
});

app.put('/api/nanotopics/:id', (req, res) => {
    const { title, content, video_url, position_index } = req.body;
    db.run(
        `UPDATE nanotopics SET title = ?, content = ?, video_url = ?, position_index = ? WHERE id = ?`,
        [title, content, video_url, position_index, req.params.id],
        function(err) {
            if (err) return res.status(500).json({ error: err.message });
            res.json({ updated: this.changes });
        }
    );
});

app.delete('/api/nanotopics/:id', (req, res) => {
    db.run(`DELETE FROM nanotopics WHERE id = ?`, req.params.id, function(err) {
        if (err) return res.status(500).json({ error: err.message });
        res.json({ deleted: this.changes });
    });
});

// Root Endpoint
app.get('/', (req, res) => {
    res.send("Bienvenido al API del Entorno VR Nano. Endpoints disponibles: /api/scientists, /api/nanotopics");
});

// Iniciar servidor
app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});
