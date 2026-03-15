const sqlite3 = require('sqlite3').verbose();
const path = require('path');
require('dotenv').config();

const dbFile = process.env.DB_FILE || 'database.sqlite';
const dbPath = path.resolve(__dirname, dbFile);

const db = new sqlite3.Database(dbPath, (err) => {
    if (err) {
        console.error('Error connecting to SQLite database:', err.message);
    } else {
        console.log('Connected to SQLite database.');
        initDb();
    }
});

function initDb() {
    // Definimos las tablas necesarias. Basado en el `MuseumMaster` clásico de Unity.
    
    // Tabla para Científicos (Paneles holográficos de personas)
    db.run(`CREATE TABLE IF NOT EXISTS scientists (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT NOT NULL,
        description TEXT NOT NULL,
        image_url TEXT,
        position_index INTEGER DEFAULT 0
    )`);

    // Tabla para Temas Nano (Paneles educativos del museo)
    db.run(`CREATE TABLE IF NOT EXISTS nanotopics (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL,
        content TEXT NOT NULL,
        video_url TEXT,
        position_index INTEGER DEFAULT 0
    )`);
    
    // Podemos pre-poblar datos de ejemplo si las tablas están vacías
    db.get("SELECT COUNT(*) as count FROM scientists", [], (err, row) => {
        if (!err && row.count === 0) {
            console.log("Pre-populating scientists data...");
            db.run(`INSERT INTO scientists (name, description, image_url, position_index) VALUES 
                ('Richard Feynman', 'Padre de la nanotecnología y creador de la frase "There\\'s plenty of room at the bottom".', 'feynman.png', 0),
                ('Eric Drexler', 'Popularizador de la nanotecnología molecular.', 'drexler.png', 1)`);
        }
    });

    db.get("SELECT COUNT(*) as count FROM nanotopics", [], (err, row) => {
        if (!err && row.count === 0) {
            console.log("Pre-populating nanotopics data...");
            db.run(`INSERT INTO nanotopics (title, content, video_url, position_index) VALUES 
                ('Grafeno', 'Material bidimensional compuesto de carbono puro con increíbles propiedades mecánicas.', 'graphene_vid.mp4', 0),
                ('Nanotubos de Carbono', 'Estructuras cilíndricas de carbono usadas en la creación de materiales ultra resistentes.', 'nanotube_vid.mp4', 1)`);
        }
    });
}

module.exports = db;
