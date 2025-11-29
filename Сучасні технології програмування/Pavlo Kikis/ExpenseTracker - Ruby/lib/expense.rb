class Expense
  attr_reader :id, :amount, :category, :date, :description, :tags, :payment_method

  def initialize(id:, amount:, category:, date: Time.now.strftime("%Y-%m-%d"),
                 description: "", tags: [], payment_method: "cash")
    @id = id
    @amount = amount.to_f.round(2)
    @category = category.capitalize
    @date = date
    @description = description
    @tags = tags.map(&:downcase)
    @payment_method = payment_method.downcase
  end

  def to_hash
    {
      id: @id,
      amount: @amount,
      category: @category,
      date: @date,
      description: @description,
      tags: @tags,
      payment_method: @payment_method
    }
  end

  def to_s
    "#{@id}: #{@date} | #{@amount} грн | #{@category} | #{@description} #{'#' + @tags.join(' #') unless @tags.empty?}"
  end
end