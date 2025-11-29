require_relative 'lib/expense_tracker'

tracker = ExpenseTracker.new
system('clear') || system('cls')
puts "Expense Tracker"

loop do
  puts "\n" + "="*50
  puts "1. Додати витрату    | 6. Пошук"
  puts "2. Всі витрати       | 7. Фільтр за датою"
  puts "3. Статистика за категоріями"
  puts "4. Статистика за місяцями"
  puts "5. Загальна сума: #{tracker.total} грн"
  puts "8. Видалити витрату  | 9. Експорт у CSV"
  puts "0. Вихід"
  print "\nВиберіть: "
  choice = gets.chomp

  case choice
  when "1" then tracker.add
  when "2" then tracker.list
  when "3" then tracker.stats_by_category
  when "4" then tracker.stats_by_month
  when "5" then puts "Загальна сума: #{tracker.total} грн"
  when "6" then tracker.search
  when "7" then tracker.filter_by_date
  when "8" then tracker.delete
  when "9" then tracker.export_csv
  when "0", "exit" then break
  else puts "Невірно"
  end
end

puts "До зустрічі!"