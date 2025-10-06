# 🚀 Quick Start Guide

Follow these simple steps to get the TODO List application running:

## Step 1: Start the Backend API

```powershell
# Open a terminal and navigate to the api folder
cd api

# Run the .NET API
dotnet run
```

✅ The API will start at: **https://localhost:7039**  
✅ Swagger UI available at: **https://localhost:7039**

## Step 2: Start the Frontend

```powershell
# Open a NEW terminal and navigate to the Angular folder
cd todo-angular

# Start the Angular development server
npm start
```

✅ The app will start at: **http://localhost:4200**  
✅ Your browser should open automatically!

## Step 3: Use the Application

1. **View** your TODO list (3 sample items are pre-loaded)
2. **Add** new items using the input field at the top
3. **Check** the checkbox to mark items as complete
4. **Double-click** on a TODO to edit it inline
5. **Click** the trash icon (🗑️) to delete an item
6. **Click** the refresh button (🔄) to reload the list

## Troubleshooting

### Backend Issues

**Problem:** Port already in use  
**Solution:** Edit `api/Properties/launchSettings.json` to change the port

**Problem:** Cannot access API from Angular  
**Solution:** Check CORS settings in `api/Program.cs`

### Frontend Issues

**Problem:** `npm install` fails  
**Solution:** 
```powershell
# Clear npm cache and retry
npm cache clean --force
npm install
```

**Problem:** Cannot connect to API  
**Solution:** Update the API URL in `todo-angular/src/app/services/todo.service.ts`

## Running Tests

### Backend Tests
```powershell
cd api.Tests
dotnet test
```

### Frontend Tests
```powershell
cd todo-angular
npm test
```

## Next Steps

- ✅ Add database persistence (Entity Framework Core)
- ✅ Add user authentication (JWT/Identity)
- ✅ Add pagination for large lists
- ✅ Add search and filtering
- ✅ Deploy to Azure/AWS

---

**Need Help?** Check the main [README.md](README.md) for detailed information.
