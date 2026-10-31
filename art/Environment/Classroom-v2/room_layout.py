"""Shared drafting coordinates. These are relative layout units, not building measurements."""

COLUMNS = [(8, 1.0), (7, 2.0), (6, 3.6), (5, 4.6),
           (4, 6.2), (3, 7.2), (2, 8.8), (1, 9.8)]
DESKS = [{'column': col, 'row': row, 'x': x, 'y': 3.15 + (row - 1) * 1.85}
         for row in range(1, 7) for col, x in COLUMNS]
GEOMETRY = {
    'room': [10.8, 15.2, 3.2],
    'regular_desks': DESKS,
    'lectern': [4.95, 2.225, 1.8, 1.1],
    'ling_he_new_desk': [3.6, 2.225, .9, 1.1],
    'li_wanyun': {'column': 6, 'row': 1},
    'front_door': [10.8, 2.225],
    'back_door': [10.8, 14.25],
}


def verify():
    assert len(DESKS) == 48
    assert {(d['column'], d['row']) for d in DESKS} == {
        (c, r) for c in range(1, 9) for r in range(1, 7)}
    for index in range(0, 8, 2):
        assert abs(COLUMNS[index + 1][1] - COLUMNS[index][1] - 1) < 1e-8
    girl = next(d for d in DESKS if d['column'] == 6 and d['row'] == 1)
    bx, by, bw, bd = GEOMETRY['ling_he_new_desk']
    lx, ly, lw, ld = GEOMETRY['lectern']
    assert abs(bx - girl['x']) < 1e-8
    assert abs(by + bd / 2 - (girl['y'] - .75 / 2)) < 1e-8
    assert abs(bx + bw / 2 - (lx - lw / 2)) < 1e-8
    assert by == ly == GEOMETRY['front_door'][1]
    assert GEOMETRY['front_door'][0] == GEOMETRY['back_door'][0] == GEOMETRY['room'][0]
    assert GEOMETRY['back_door'][1] - .675 > max(d['y'] + 1.0025 for d in DESKS)


verify()
