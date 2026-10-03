# OpenSpace

OpenSpace is a read-only, local-first Windows storage analyzer. It scans a chosen folder, ranks entries by size, supports cancellation, and never deletes or modifies files.

Version 1.0 uses a fault-tolerant directory walker: inaccessible entries and reparse points are reported and skipped instead of aborting the entire scan. Directory totals use boundary-safe path checks, and detailed results include scanned-file counts, total bytes, progress events, and skipped-item evidence.

MIT licensed.
