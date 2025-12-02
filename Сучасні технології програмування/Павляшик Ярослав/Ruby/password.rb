require 'io/console'

class PasswordGenerator
  LOWERCASE = ('a'..'z').to_a.freeze
  UPPERCASE = ('A'..'Z').to_a.freeze
  DIGITS = ('0'..'9').to_a.freeze
  SPECIAL_CHARS = %w[! @ # $ % ^ & * ( ) _ + - = [ ] { } ; : < > ?].freeze

  def initialize
    @custom_sets = []
    @mandatory_strings = []
  end

  def add_mandatory_string(str)
    if str.nil? || str.empty?
      str = (LOWERCASE + UPPERCASE + DIGITS + SPECIAL_CHARS).sample
    end
    @mandatory_strings << str
    @custom_sets << str.chars
  end

  def generate_password(length, options)
    length = [length.to_i, 8].max
    full_char_pool = get_standard_char_set(options).dup
    @custom_sets.each { |set| full_char_pool.concat(set) }
    full_char_pool.uniq!
    return nil if full_char_pool.empty?

    password_chars = []
    ensure_complexity(password_chars, options)

    mandatory_length = @mandatory_strings.map(&:length).sum
    remaining_length = length - password_chars.join.length - mandatory_length
    remaining_length.times { password_chars << full_char_pool.sample } if remaining_length > 0

    if @mandatory_strings.empty?
      password_chars.shuffle.join
    else
      combined = password_chars.shuffle
      insertion_index = rand(0..combined.length)
      combined.insert(insertion_index, @mandatory_strings.join)
      combined.join
    end
  end

  private

  def get_standard_char_set(options)
    set = LOWERCASE.dup
    set.concat(UPPERCASE) if options[:use_upper]
    set.concat(DIGITS) if options[:use_digits]
    set.concat(SPECIAL_CHARS) if options[:use_special]
    set
  end

  def ensure_complexity(password_chars, options)
    password_chars << LOWERCASE.sample
    password_chars << UPPERCASE.sample if options[:use_upper]
    password_chars << DIGITS.sample if options[:use_digits]
    password_chars << SPECIAL_CHARS.sample if options[:use_special]
  end
end

generator = PasswordGenerator.new

puts "Введіть слово або символ для пароля (натискання Enter не обов'язкове):"

input_chars = []
while true
  char = STDIN.getch
  break if char == "\r" || char == "\n" # Enter завершує ввід
  break if char.empty? # нічого не натискаючи — вихід
  input_chars << char
  print char # для відображення введеного символу
end

mandatory_input = input_chars.join
generator.add_mandatory_string(mandatory_input)

PASSWORD_LENGTH = 16
GENERATION_OPTIONS = { use_upper: true, use_digits: true, use_special: true }

password = generator.generate_password(PASSWORD_LENGTH, GENERATION_OPTIONS)
puts "\nЗгенерований пароль: #{password}"
