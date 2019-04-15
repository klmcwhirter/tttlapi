['Incomplete', 'X Won', 'O Won', 'Tie']
    .forEach((l, r) =>
        print(l + ': ' + db.games.find({ Result: r }).count())
    );
print('Total' + ': ' + db.games.find().count());
