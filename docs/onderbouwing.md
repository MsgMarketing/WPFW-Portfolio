# Onderbouwing & Request/Response-cyclus (Opdracht 1 WPFW)

**Student:** Muhammed Gezer  
**Datum:** 22 september 2026  
**Vak:** Webprogramming, Frameworks & Usability (WPFW)  

---

## 1. Request / Response-cyclus

Onderstaand schema toont wat er gebeurt wanneer een client een statische pagina van het portfolio opvraagt:

```text
+----------+                                      +--------------------+
|  Client  |                                      |   Webserver (NL)   |
| (Browser)|                                      |    (Statisch)      |
+----+-----+                                      +---------+----------+
     |                                                      |
     |  1. DNS Lookup (resolvet domein naar IP-adres)       |
     |----------------------------------------------------->|
     |                                                      |
     |  2. TCP 3-Way Handshake (SYN, SYN-ACK, ACK) + TLS    |
     |<====================================================>|
     |                                                      |
     |  3. HTTP GET /index.html                             |
     |----------------------------------------------------->|
     |                                                      |
     |  4. HTTP/1.1 200 OK (Content-Type: text/html)        |
     |     [HTML payload wordt ontvangen]                   |
     |<-----------------------------------------------------|
     |                                                      |
     |  5. Browser parseert HTML, ontdekt <link rel="stylesheet">
     |                                                      |
     |  6. HTTP GET /css/style.css                          |
     |----------------------------------------------------->|
     |                                                      |
     |  7. HTTP/1.1 200 OK (Content-Type: text/css)         |
     |<-----------------------------------------------------|
     |                                                      |
     |  8. DOM + CSSOM -> Render Tree -> Layout -> Paint    |
     v                                                      v