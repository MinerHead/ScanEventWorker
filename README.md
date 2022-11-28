# ScanEventWorker
This is a .Net 6.0 Worker Service Application consuming a scan event API (GET http://localhost/v1/scans/scanevents) and recording the last event made against a parcel and any pickup or delivery times. Detailed Spec can be found in ScanEventExercise.pdf

## Additional Tools Required
DB Browser for Sqlite db

## Assumptions
1. API server is always active
2. EventId and CreatedDateTimeUtc values are not monotonic related
3. Type values are case insensitive
4. The term "last event" refers to the event with latest CreatedDateTimeUtc value
5. All processed data should be saved in a offline sqlite database

## Improvements
1. Add unit testing
2. Minimize database IO by grouping reads and write and storing reads data for later use
3. Implement transactions to prevent data distortion
4. Use parallel Tasks to speed up data process
5. Implement data archiving to avoid large data set slows down performance
6. In order for a downstream worker to also perform actions against the processed events, an API layer needs to be added into this worker. Hence any downstream workers denpending on this upstream worker can get the data stored the sqlite db.

