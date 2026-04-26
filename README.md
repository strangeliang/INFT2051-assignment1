# Parcel Station

Parcel Station is a mobile app prototype built with .NET MAUI for INFT2051 Assignment 3. The app helps users manage parcel collection by allowing them to register, log in, add parcel records, search parcel details, scan parcel QR codes, and view parcel history.

## Main Features

- User registration and login
- Main dashboard page
- Add new parcel records
- Search parcels by parcel code
- View parcel details and QR code
- QR code scanning for parcel lookup
- Parcel history page
- Mark parcels as Collected
- Clear parcel records from history
- Dashboard statistics for Pending, Ready, and Collected parcels
- Recent parcel preview
- Vibration feedback
- Android success beep feedback
- Local SQLite database storage

## Technologies Used

- C#
- .NET MAUI
- XAML
- SQLite
- ZXing.Net.MAUI
- CommunityToolkit.Maui
- Syncfusion.Maui.Toolkit

## Database

The app uses a local SQLite database to store user and parcel information. Each parcel record is linked to the logged-in username, so users can only view and manage their own parcel records.

## Navigation Flow

Login Page → Dashboard  
Login Page → Register Page  
Dashboard → Add Parcel Page  
Dashboard → Search Result Page  
Dashboard → Scan Page  
Dashboard → History Page  
History Page → Mark as Collected / Clear Record

## Testing Status

The app has been tested on Windows Machine. The tested features include registration, login, parcel adding, parcel search, result display, QR scanning, dashboard statistics, parcel history, mark as collected, and clear parcel record.

## Final Submission Notes

This repository contains the final app source code for INFT2051 Assignment 3. The app focuses on a parcel collection workflow with local storage, QR scanning, user feedback, and parcel history management.
