import http from 'http';

const BASE_URL = process.env.BASE_URL || 'http://localhost:5193';
let passed = 0;
let failed = 0;
const results = [];

function checkUrl(name, url, expectedStatus = 200) {
  return new Promise(resolve => {
    const fullUrl = `${BASE_URL}${url}`;
    http.get(fullUrl, res => {
      let body = '';
      res.on('data', chunk => body += chunk);
      res.on('end', () => {
        if (res.statusCode === expectedStatus) {
          results.push({ name, status: 'PASS', detail: `${res.statusCode} ${res.statusMessage}` });
          passed++;
        } else {
          results.push({ name, status: 'FAIL', detail: `Expected ${expectedStatus}, got ${res.statusCode}` });
          failed++;
        }
        resolve();
      });
    }).on('error', err => {
      results.push({ name, status: 'FAIL', detail: `Error: ${err.message}` });
      failed++;
      resolve();
    });
  });
}

async function checkContent(url, keyword) {
  return new Promise(resolve => {
    http.get(`${BASE_URL}${url}`, res => {
      let body = '';
      res.on('data', chunk => body += chunk);
      res.on('end', () => {
        if (body.includes(keyword)) {
          results.push({ name: `首页包含关键字"${keyword}"`, status: 'PASS', detail: 'Keyword matched' });
          passed++;
        } else {
          results.push({ name: `首页包含关键字"${keyword}"`, status: 'FAIL', detail: 'Keyword not found' });
          failed++;
        }
        resolve();
      });
    }).on('error', err => {
      results.push({ name: `首页包含关键字"${keyword}"`, status: 'FAIL', detail: `Error: ${err.message}` });
      failed++;
      resolve();
    });
  });
}

console.log('=== Smoke Test - Library Seat Reservation System ===');
console.log(`Base URL: ${BASE_URL}\n`);

const checks = [
  checkUrl('Homepage (GET /)', '/'),
  checkContent('/', '图书馆座位预约系统'),
  checkUrl('Seats (GET /Seats - not logged in)', '/Seats', 302),
  checkUrl('Admin Login (GET /Admin/Login)', '/Admin/Login'),
  checkUrl('Admin Reservations (GET /Admin/Reservations - not logged in)', '/Admin/Reservations', 302),
  checkUrl('Admin Statistics (GET /Admin/Statistics - not logged in)', '/Admin/Statistics', 302),
  checkUrl('Admin Seats (GET /Admin/Seats - not logged in)', '/Admin/Seats', 302),
  checkUrl('Bootstrap CSS (static resource)', '/lib/bootstrap/dist/css/bootstrap.min.css'),
  checkUrl('My Reservations (GET /Reservations/My - not logged in)', '/Reservations/My', 302),
];

await Promise.all(checks);

console.log('=== Results ===');
console.table(results);
console.log(`\nTotal: ${passed + failed} | Passed: ${passed} | Failed: ${failed}`);

if (failed > 0) process.exit(1);
