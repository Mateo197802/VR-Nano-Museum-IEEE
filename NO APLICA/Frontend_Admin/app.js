// Configuración del servidor
const API_BASE_URL = 'http://localhost:3000/api';
let currentTab = 'scientists'; // Puede ser 'scientists' o 'nanotopics'

// Elementos del DOM
const form = document.getElementById('data-form');
const tableBody = document.getElementById('data-table-body');
const inputId = document.getElementById('item-id');
const inputTitle = document.getElementById('title');
const inputDesc = document.getElementById('description');
const inputMedia = document.getElementById('media_url');

// Etiquetas Dinámicas
const labelTitle = document.getElementById('label-title');
const labelMedia = document.getElementById('label-media');
const formTitle = document.getElementById('form-title');
const tableTitle = document.getElementById('table-title');
const colTitle = document.getElementById('col-title');
const colMedia = document.getElementById('col-media');

// Iniciar
document.addEventListener('DOMContentLoaded', () => {
    switchTab('scientists');
});

function switchTab(tab) {
    currentTab = tab;
    
    // UI - Tabs
    document.getElementById('tab-scientists').className = tab === 'scientists' ? 'tab-active py-4 px-1 text-sm font-medium' : 'tab-inactive py-4 px-1 text-sm font-medium';
    document.getElementById('tab-nanotopics').className = tab === 'nanotopics' ? 'tab-active py-4 px-1 text-sm font-medium' : 'tab-inactive py-4 px-1 text-sm font-medium';
    
    // UI - Textos Formulario
    if(tab === 'scientists') {
        formTitle.innerText = "Añadir Nuevo Holograma Scientist";
        tableTitle.innerText = "Listado de Científicos Activos";
        labelTitle.innerText = "Nombre del Científico";
        labelMedia.innerText = "Archivo de Imagen (URL o nombre local)";
        colTitle.innerText = "Nombre";
        colMedia.innerText = "Imagen";
    } else {
        formTitle.innerText = "Añadir Nuevo Panel Nano";
        tableTitle.innerText = "Listado de Paneles Educativos Activos";
        labelTitle.innerText = "Título del Panel";
        labelMedia.innerText = "Video URL (o nombre local mp4)";
        colTitle.innerText = "Título";
        colMedia.innerText = "Video";
    }
    
    resetForm();
    fetchData();
}

async function fetchData() {
    tableBody.innerHTML = '<tr><td colspan="5" class="px-6 py-4 text-center text-sm text-gray-500">Cargando...</td></tr>';
    
    try {
        const response = await fetch(`${API_BASE_URL}/${currentTab}`);
        if (!response.ok) throw new Error('Error en HTTP: ' + response.status);
        const data = await response.json();
        
        renderTable(data);
    } catch (error) {
        tableBody.innerHTML = `<tr><td colspan="5" class="px-6 py-4 text-center text-sm text-red-500">Error al contactar al servidor: ${error.message}. ¿Está corriendo node server.js?</td></tr>`;
    }
}

function renderTable(data) {
    tableBody.innerHTML = '';
    
    if (data.length === 0) {
        tableBody.innerHTML = `<tr><td colspan="5" class="px-6 py-4 text-center text-sm text-gray-500">No hay registros en la base de datos.</td></tr>`;
        return;
    }

    data.forEach(item => {
        const titleText = currentTab === 'scientists' ? item.name : item.title;
        const mediaText = currentTab === 'scientists' ? item.image_url : item.video_url;
        const descText = currentTab === 'scientists' ? item.description : item.content;
        
        const tr = document.createElement('tr');
        tr.className = "hover:bg-gray-50";
        tr.innerHTML = `
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">${item.id}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">${titleText}</td>
            <td class="px-6 py-4 text-sm text-gray-500 max-w-xs truncate" title="${descText}">${descText}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">${mediaText || '-'}</td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button onclick='editItem(${JSON.stringify(item)})' class="text-blue-600 hover:text-blue-900 mr-3"><i class="fa-solid fa-pen"></i></button>
                <button onclick="deleteItem(${item.id})" class="text-red-600 hover:text-red-900"><i class="fa-solid fa-trash"></i></button>
            </td>
        `;
        tableBody.appendChild(tr);
    });
}

function resetForm() {
    form.reset();
    inputId.value = '';
    if(currentTab === 'scientists') {
        formTitle.innerText = "Añadir Nuevo Holograma Scientist";
    } else {
        formTitle.innerText = "Añadir Nuevo Panel Nano";
    }
}

function editItem(item) {
    inputId.value = item.id;
    
    if(currentTab === 'scientists') {
        inputTitle.value = item.name;
        inputDesc.value = item.description;
        inputMedia.value = item.image_url;
        formTitle.innerText = "Editando Holograma (ID: " + item.id + ")";
    } else {
        inputTitle.value = item.title;
        inputDesc.value = item.content;
        inputMedia.value = item.video_url;
        formTitle.innerText = "Editando Panel Nano (ID: " + item.id + ")";
    }
    
    // Scroll al form
    document.querySelector('header').scrollIntoView({ behavior: 'smooth' });
}

async function deleteItem(id) {
    const result = await Swal.fire({
        title: '¿Estás seguro?',
        text: "¡No podrás revertir esto!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, borrarlo'
    });

    if (result.isConfirmed) {
        try {
            const response = await fetch(`${API_BASE_URL}/${currentTab}/${id}`, {
                method: 'DELETE'
            });
            if (!response.ok) throw new Error('Error al borrar');
            
            Swal.fire('Borrado!', 'El registro ha sido eliminado.', 'success');
            fetchData();
        } catch (error) {
            Swal.fire('Error', error.message, 'error');
        }
    }
}

form.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const id = inputId.value;
    const bodyData = currentTab === 'scientists' 
        ? {
            name: inputTitle.value,
            description: inputDesc.value,
            image_url: inputMedia.value
        }
        : {
            title: inputTitle.value,
            content: inputDesc.value,
            video_url: inputMedia.value
        };

    const method = id ? 'PUT' : 'POST';
    const url = id ? `${API_BASE_URL}/${currentTab}/${id}` : `${API_BASE_URL}/${currentTab}`;

    try {
        const response = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(bodyData)
        });

        if (!response.ok) throw new Error('Error en HTTP');
        
        Swal.fire({
            icon: 'success',
            title: id ? 'Actualizado' : 'Creado',
            text: 'Registro guardado exitosamente.',
            timer: 1500,
            showConfirmButton: false
        });
        
        resetForm();
        fetchData();
        
    } catch (error) {
        Swal.fire('Error', 'No se pudo guardar: ' + error.message, 'error');
    }
});
