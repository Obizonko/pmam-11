require 'json'
require_relative 'expense'

class ExpenseTracker
  FILE_PATH = File.expand_path('../../storage/expenses.json', __FILE__)
  CATEGORIES = %w[Їжа Транспорт Розваги Комуналка Здоров'я Освіта Інше]

  def initialize
    @expenses = load_expenses
    @next_id = @expenses.map(&:id).max.to_i + 1
  end

  def add
    print "Сума: "
    amount = gets.chomp.to_f
    return puts "Невірна сума" if amount <= 0

    puts "Оберіть категорію:"
    CATEGORIES.each_with_index { |c, i| puts "#{i+1}. #{c}" }
    print "або введіть свою: "
    cat_input = gets.chomp
    category = cat_input.to_i.between?(1, CATEGORIES.size) ? CATEGORIES[cat_input.to_i - 1] : cat_input.capitalize

    print "Дата (YYYY-MM-DD, Enter = сьогодні): "
    date = gets.chomp.strip
    date = date.empty? ? Time.now.strftime("%Y-%m-%d") : date

    print "Опис: "
    description = gets.chomp

    print "Теги (через пробіл): "
    tags = gets.chomp.split.map(&:strip).reject(&:empty?)

    print "Спосіб оплати (cash/card): "
    payment = gets.chomp.downcase
    payment = %w[cash card].include?(payment) ? payment : "cash"

    expense = Expense.new(
      id: @next_id,
      amount: amount,
      category: category,
      date: date,
      description: description,
      tags: tags,
      payment_method: payment
    )
    @expenses << expense
    @next_id += 1
    save
    puts "Витрату додано!"
  end

  def list
    if @expenses.empty?
      puts "Немає витрат"
    else
      @expenses.sort_by(&:date).reverse.each { |e| puts e }
    end
  end

  def total
    @expenses.sum(&:amount).round(2)
  end

  def stats_by_category
    stats = @expenses.group_by(&:category).transform_values { |exps| exps.sum(&:amount).round(2) }
    stats.sort_by { |_, sum| -sum }.each { |cat, sum| puts "#{cat}: #{sum} грн" }
  end

  def stats_by_month
    grouped = @expenses.group_by { |e| e.date[0..6] }
    grouped.sort.reverse.each { |month, exps| puts "#{month}: #{exps.sum(&:amount).round(2)} грн" }
  end

  def search
    print "Пошук (текст/тег/категорія): "
    query = gets.chomp.downcase
    results = @expenses.select do |e|
      e.description.downcase.include?(query) ||
      e.category.downcase.include?(query) ||
      e.tags.any? { |t| t.include?(query) }
    end
    results.each { |e| puts e }
    puts "Знайдено: #{results.size}"
  end

  def filter_by_date
    print "З дати (YYYY-MM-DD): "
    from = gets.chomp
    print "По дату (YYYY-MM-DD): "
    to = gets.chomp
    filtered = @expenses.select { |e| e.date >= from && e.date <= to }
    filtered.each { |e| puts e }
    puts "Загалом: #{filtered.sum(&:amount).round(2)} грн"
  end

  def delete
    print "ID для видалення: "
    id = gets.chomp.to_i
    if @expenses.any? { |e| e.id == id }
      @expenses.reject! { |e| e.id == id }
      save
      puts "Видалено"
    else
      puts "Не знайдено"
    end
  end

  def export_csv
    return puts "Немає даних" if @expenses.empty?
    File.write("storage/export_#{Time.now.strftime('%Y%m%d')}.csv",
               "ID,Дата,Сума,Категорія,Опис,Теги,Оплата\n" +
               @expenses.map { |e|
                 "#{e.id},#{e.date},#{e.amount},#{e.category},\"#{e.description}\",\"#{e.tags.join(',')}\",#{e.payment_method}"
               }.join("\n"))
    puts "Експортовано в CSV"
  end

  private

  def load_expenses
    return [] unless File.exist?(FILE_PATH)
    json = File.read(FILE_PATH)
    return [] if json.empty?
    data = JSON.parse(json)
    data.map do |h|
      Expense.new(
        id: h["id"],
        amount: h["amount"],
        category: h["category"],
        date: h["date"],
        description: h["description"] || "",
        tags: h["tags"] || [],
        payment_method: h["payment_method"] || "cash"
      )
    end
  end

  def save
    data = @expenses.map(&:to_hash)
    File.write(FILE_PATH, JSON.pretty_generate(data))
  end
end