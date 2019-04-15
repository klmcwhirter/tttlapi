labels = ['Incomplete', 'X Won', 'O Won', 'Tie'];
[...Array(4).keys()]
    .forEach(r =>
        print(labels[r] + ': ' + db.games.find({ Result: r }).count())
    );
print('Total' + ': ' + db.games.find().count());
