# SIH1329 — REST API Documentation

**Base URL:** `http://localhost:5000/api` or `https://localhost:7106/api`  
**API Specification:** OpenAPI 3.0 / Swagger UI at `/swagger`  
**Authentication Scheme:** ASP.NET Core Cookie Authentication & Identity Bearer Tokens  

---

## 1. Authentication Endpoints (`/api/auth`)

### 1.1 Citizen Registration
- **Endpoint:** `POST /api/auth/register`
- **Access:** Public
- **Request Body:**
```json
{
  "fullName": "Aarav Sharma",
  "email": "aarav.sharma@example.com",
  "password": "Citizen@123456",
  "confirmPassword": "Citizen@123456",
  "state": "Maharashtra",
  "language": "Marathi",
  "ageGroup": "26-35",
  "gender": "Male",
  "occupation": "Software Engineer"
}
```
- **Responses:**
  - `200 OK`: Account created successfully.
  - `400 Bad Request`: Validation failure or password requirements not met.
  - `409 Conflict`: Email already exists.

### 1.2 User Login
- **Endpoint:** `POST /api/auth/login`
- **Access:** Public
- **Request Body:**
```json
{
  "email": "admin@govfeedback.in",
  "password": "Admin@123456",
  "rememberMe": true
}
```
- **Responses:**
  - `200 OK`: Returns authenticated session details and roles (`["Admin"]`).
  - `401 Unauthorized`: Invalid credentials.

---

## 2. News Endpoints (`/api/news`)

### 2.1 Get Paginated News Stories
- **Endpoint:** `GET /api/news`
- **Access:** Public
- **Query Parameters:**
  - `search` (string, optional): Keyword search in title/summary.
  - `categoryId` (int, optional): Filter by sector category.
  - `state` (string, optional): Filter by Indian State/UT.
  - `language` (string, optional): Filter by language.
  - `page` (int, default: 1): Page number.
  - `pageSize` (int, default: 20): Items per page.
- **Response `200 OK`:**
```json
{
  "data": [
    {
      "id": 1,
      "title": "National Highway Expansion: 500 km New Roads Approved",
      "summary": "500 km of highways approved under Bharatmala Phase IV.",
      "state": "Maharashtra",
      "language": "English",
      "mediaSource": "PIB Mumbai",
      "categoryId": 1,
      "isPublished": true,
      "publishedDate": "2026-11-20T10:00:00Z"
    }
  ],
  "total": 15,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

### 2.2 Get News Story by ID
- **Endpoint:** `GET /api/news/{id}`
- **Access:** Public
- **Responses:**
  - `200 OK`: Returns news story details including category navigation object.
  - `404 Not Found`: Story does not exist.

### 2.3 Create News Story
- **Endpoint:** `POST /api/news`
- **Access:** Admin only (`[Authorize(Roles = "Admin")]`)
- **Request Body:**
```json
{
  "title": "PM Vishwakarma Scheme Crosses 20 Lakh Beneficiaries",
  "content": "Full article description...",
  "summary": "Executive summary...",
  "categoryId": 8,
  "state": "Gujarat",
  "language": "Gujarati",
  "mediaSource": "Gujarat Samachar",
  "isPublished": true
}
```
- **Response `201 Created`**: Returns created object with assigned ID.

### 2.4 Update News Story
- **Endpoint:** `PUT /api/news/{id}`
- **Access:** Admin only
- **Response `200 OK`**: Returns updated entity.

### 2.5 Delete News Story
- **Endpoint:** `DELETE /api/news/{id}`
- **Access:** Admin only
- **Response `204 No Content`**: Deletion confirmed.

---

## 3. Feedback Endpoints (`/api/feedback`)

### 3.1 Get Feedback for News Story
- **Endpoint:** `GET /api/feedback?newsId=1&count=10`
- **Access:** Public
- **Response `200 OK`**: Returns list of approved, non-deleted feedbacks.

### 3.2 Get Feedback Statistics for News Story
- **Endpoint:** `GET /api/feedback/stats/{newsId}`
- **Access:** Public
- **Response `200 OK`:**
```json
{
  "totalFeedbacks": 12,
  "averageRating": 4.2,
  "positiveCount": 8,
  "neutralCount": 3,
  "negativeCount": 1,
  "positivePercentage": 66.7,
  "neutralPercentage": 25.0,
  "negativePercentage": 8.3,
  "ratingDistribution": {
    "1": 0,
    "2": 1,
    "3": 3,
    "4": 5,
    "5": 3
  }
}
```

### 3.3 Submit 360° Feedback
- **Endpoint:** `POST /api/feedback`
- **Access:** Authenticated Citizen
- **Request Body:**
```json
{
  "newsStoryId": 1,
  "rating": 5,
  "userSentiment": 1,
  "feedbackCategory": "Policy Impact",
  "comment": "Excellent connectivity improvement for freight logistics.",
  "suggestions": "Ensure pedestrian overpasses in densely populated talukas."
}
```
- **Responses:**
  - `201 Created`: Feedback recorded.
  - `409 Conflict`: Citizen has already submitted feedback for this story.

---

## 4. Analytics Endpoints (`/api/analytics`)

### 4.1 Platform Overview
- **Endpoint:** `GET /api/analytics/overview`
- **Response `200 OK`:**
```json
{
  "totalUsers": 20,
  "totalNews": 15,
  "totalFeedbacks": 55,
  "averageRating": 3.8,
  "positiveFeedbacks": 28,
  "neutralFeedbacks": 18,
  "negativeFeedbacks": 9,
  "totalStates": 6,
  "totalLanguages": 5
}
```

### 4.2 Sentiment Analysis
- **Endpoint:** `GET /api/analytics/sentiment?state=Maharashtra`
- **Response `200 OK`:**
```json
{
  "positive": 28,
  "neutral": 18,
  "negative": 9,
  "total": 55,
  "positivePercent": 50.9,
  "neutralPercent": 32.7,
  "negativePercent": 16.4
}
```

### 4.3 Regional Distribution
- **Endpoint:** `GET /api/analytics/regional`
- **Response `200 OK`:**
```json
[
  { "state": "Maharashtra", "newsCount": 3, "feedbackCount": 18, "averageRating": 4.1 },
  { "state": "Delhi", "newsCount": 2, "feedbackCount": 14, "averageRating": 3.9 }
]
```

### 4.4 Monthly Trends
- **Endpoint:** `GET /api/analytics/monthly?months=6`
- **Response `200 OK`:** Returns monthly feedback and news publishing counts.
